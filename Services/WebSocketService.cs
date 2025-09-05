using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using OrderBook.Hubs;
using OrderBook.Models;  // Add this line to import OrderBookData

namespace OrderBook.Services
{
  public class WebSocketService : IHostedService, IDisposable
  {
    private readonly IConfiguration _configuration;
    private readonly IHubContext<OrderBookHub> _hubContext;
    private readonly ILogger<WebSocketService> _logger;
    private readonly IOrderBookAuditServiceFactory _auditServiceFactory;
    private readonly IOrderBookAuditService _auditService;
    private ClientWebSocket _webSocket;
    private bool _running;
    private bool _disposed;
    private readonly SemaphoreSlim _reconnectLock = new(1, 1);

    public WebSocketService(
        IConfiguration configuration,
        IHubContext<OrderBookHub> hubContext,
        ILogger<WebSocketService> logger,
        IOrderBookAuditServiceFactory auditServiceFactory)
    {
      _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
      _hubContext = hubContext ?? throw new ArgumentNullException(nameof(hubContext));
      _logger = logger ?? throw new ArgumentNullException(nameof(logger));
      _auditServiceFactory = auditServiceFactory ?? throw new ArgumentNullException(nameof(auditServiceFactory));
      _auditService = auditServiceFactory.Create("BTC-EUR");
      _webSocket = new ClientWebSocket();
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
      if (_disposed)
        throw new ObjectDisposedException(nameof(WebSocketService));

      _running = true;
      await ConnectAndListen(cancellationToken);
    }

    private async Task ConnectAndListen(CancellationToken cancellationToken)
    {
      try
      {
        await _reconnectLock.WaitAsync(cancellationToken);

        if (_disposed)
          throw new ObjectDisposedException(nameof(WebSocketService));

        if (_webSocket.State == WebSocketState.Open)
          return;

        if (_webSocket.State != WebSocketState.None)
        {
          _webSocket.Dispose();
          _webSocket = new ClientWebSocket();
        }

        var wsUrl = _configuration["ExternalApi:WebSocketUrl"]
            ?? throw new InvalidOperationException("WebSocket URL not configured");

        await _webSocket.ConnectAsync(new Uri(wsUrl), cancellationToken);

        var subscribeMessage = JsonSerializer.Serialize(new
        {
          @event = "bts:subscribe",
          data = new { channel = "detail_order_book_btceur" }
        });

        await _webSocket.SendAsync(
            new ArraySegment<byte>(Encoding.UTF8.GetBytes(subscribeMessage)),
            WebSocketMessageType.Text,
            true,
            cancellationToken);

        _ = ReceiveLoop(cancellationToken);
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error in ConnectAndListen");
        throw;
      }
      finally
      {
        _reconnectLock.Release();
      }
    }

    private async Task ReceiveLoop(CancellationToken cancellationToken)
    {
      var buffer = new byte[8192];
      var messageBuffer = new StringBuilder();
      var reconnectDelay = TimeSpan.FromSeconds(5);

      while (_running && !cancellationToken.IsCancellationRequested && !_disposed)
      {
        try
        {
          if (_webSocket.State != WebSocketState.Open)
          {
            _logger.LogWarning("WebSocket not open, attempting to reconnect...");
            await Task.Delay(reconnectDelay, cancellationToken);
            await ConnectAndListen(cancellationToken);
            continue;
          }

          var result = await _webSocket.ReceiveAsync(
              new ArraySegment<byte>(buffer),
              cancellationToken);

          if (result.MessageType == WebSocketMessageType.Close)
          {
            _logger.LogInformation("Received close message from server");
            await HandleWebSocketClose(cancellationToken);
            continue;
          }

          messageBuffer.Append(
              Encoding.UTF8.GetString(buffer, 0, result.Count));

          if (result.EndOfMessage)
          {
            await ProcessMessage(messageBuffer.ToString(), cancellationToken);
            messageBuffer.Clear();
          }
        }
        catch (WebSocketException ex)
        {
          _logger.LogWarning(ex, "WebSocket connection error, attempting to reconnect...");
          await HandleWebSocketClose(cancellationToken);
        }
        catch (Exception ex) when (!cancellationToken.IsCancellationRequested)
        {
          _logger.LogError(ex, "Unexpected error in ReceiveLoop");
          await HandleWebSocketClose(cancellationToken);
        }
      }
    }

    private async Task HandleWebSocketClose(CancellationToken cancellationToken)
    {
      try
      {
        if (_webSocket.State == WebSocketState.Open)
        {
          await _webSocket.CloseAsync(
              WebSocketCloseStatus.NormalClosure,
              string.Empty,
              cancellationToken);
        }
      }
      catch (Exception ex)
      {
        _logger.LogWarning(ex, "Error while closing WebSocket connection");
      }

      _webSocket.Dispose();
      _webSocket = new ClientWebSocket();

      await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
      await ConnectAndListen(cancellationToken);
    }

    private async Task ProcessMessage(string message, CancellationToken cancellationToken)
    {
      try
      {
        var jsonNode = JsonNode.Parse(message);

        if (jsonNode?["event"]?.GetValue<string>() != "data" ||
            jsonNode?["channel"]?.GetValue<string>() != "detail_order_book_btceur")
        {
          return;
        }

        var orderBookData = jsonNode["data"].ToJsonString();
        var data = JsonSerializer.Deserialize<OrderBookData>(orderBookData);

        if (data != null)
        {
          await Task.WhenAll(
              _hubContext.Clients.All.SendAsync("ReceiveOrderBook", orderBookData, cancellationToken),
              _auditService.LogOrderBookSnapshot(orderBookData)
          );
        }
        else
        {
          _logger.LogError("Failed to deserialize order book data");
        }
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error processing message: {Message}", message);
      }
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
      _running = false;
      if (_webSocket.State == WebSocketState.Open)
      {
        await _webSocket.CloseAsync(
            WebSocketCloseStatus.NormalClosure,
            string.Empty,
            cancellationToken);
      }
    }

    public void Dispose()
    {
      Dispose(true);
      GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
      if (_disposed) return;

      if (disposing)
      {
        _webSocket?.Dispose();
        _reconnectLock?.Dispose();
      }

      _disposed = true;
    }
  }
}
