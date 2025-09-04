using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using OrderBook.Hubs;

namespace OrderBook.Services
{
  public class WebSocketService : IHostedService, IDisposable
  {
    private readonly IConfiguration _configuration;
    private readonly IHubContext<OrderBookHub> _hubContext;
    private readonly ILogger<WebSocketService> _logger;
    private readonly IOrderBookAuditServiceFactory _auditServiceFactory;
    private ClientWebSocket _webSocket;
    private bool _running;
    private readonly SemaphoreSlim _reconnectLock = new(1, 1);

    public WebSocketService(
        IConfiguration configuration,
        IHubContext<OrderBookHub> hubContext,
        ILogger<WebSocketService> logger,
        IOrderBookAuditServiceFactory auditServiceFactory)
    {
      _configuration = configuration;
      _hubContext = hubContext;
      _logger = logger;
      _auditServiceFactory = auditServiceFactory;
      _webSocket = new ClientWebSocket();
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
      _running = true;
      await ConnectAndListen(cancellationToken);
    }

    private async Task ConnectAndListen(CancellationToken cancellationToken)
    {
      try
      {
        await _reconnectLock.WaitAsync(cancellationToken);

        if (_webSocket.State == WebSocketState.Open)
          return;

        if (_webSocket.State != WebSocketState.None)
        {
          _webSocket.Dispose();
          _webSocket = new ClientWebSocket();
        }

        var wsUrl = _configuration["ExternalApi:WebSocketUrl"];
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

      while (_running && !cancellationToken.IsCancellationRequested)
      {
        try
        {
          var result = await _webSocket.ReceiveAsync(
              new ArraySegment<byte>(buffer),
              cancellationToken);

          if (result.MessageType == WebSocketMessageType.Close)
          {
            await _webSocket.CloseAsync(
                WebSocketCloseStatus.NormalClosure,
                string.Empty,
                cancellationToken);
            break;
          }

          messageBuffer.Append(
              Encoding.UTF8.GetString(buffer, 0, result.Count));

          if (result.EndOfMessage)
          {
            var message = messageBuffer.ToString();
            await _hubContext.Clients.All.SendAsync("ReceiveOrderBook", message, cancellationToken);

            var auditService = _auditServiceFactory.CreateScope();
            await auditService.LogOrderBookSnapshot(message);

            messageBuffer.Clear();
          }
        }
        catch (Exception ex)
        {
          _logger.LogError(ex, "Error in ReceiveLoop");
          await Task.Delay(5000, cancellationToken);
          await ConnectAndListen(cancellationToken);
        }
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
      _webSocket?.Dispose();
      _reconnectLock?.Dispose();
    }
  }
}
