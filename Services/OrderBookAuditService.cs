using System;
using System.Threading.Tasks;
using System.Text.Json;
using OrderBook.Data;
using OrderBook.Models;
using Microsoft.Extensions.Logging;

namespace OrderBook.Services
{
  public class OrderBookAuditService : IOrderBookAuditService
  {
    private readonly OrderBookContext _context;
    private readonly ILogger<OrderBookAuditService> _logger;
    private bool _disposed;

    public OrderBookAuditService(OrderBookContext context, ILogger<OrderBookAuditService> logger)
    {
      _context = context;
      _logger = logger;
    }

    public async Task LogOrderBookSnapshot(string snapshot)
    {
      try
      {
        var orderBookData = JsonSerializer.Deserialize<OrderBookData>(snapshot);

        var audit = new OrderBookAudit
        {
          Timestamp = orderBookData.Timestamp,
          Microtimestamp = orderBookData.Microtimestamp,
          RawData = snapshot,
          LoggedAt = DateTime.UtcNow
        };

        await _context.OrderBookAudits.AddAsync(audit);
        await _context.SaveChangesAsync();

        _logger.LogInformation(
            "Logged order book snapshot. Timestamp: {Timestamp}, MicroTimestamp: {MicroTimestamp}",
            audit.Timestamp,
            audit.Microtimestamp);
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error logging order book snapshot: {Message}", ex.Message);
        throw;
      }
    }

    public void Dispose()
    {
      Dispose(true);
      GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
      if (!_disposed)
      {
        if (disposing)
        {
          _context?.Dispose();
        }
        _disposed = true;
      }
    }
  }
}