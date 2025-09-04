using System;
using System.Threading.Tasks;

namespace OrderBook.Services
{
  public interface IOrderBookAuditService : IDisposable
  {
    Task LogOrderBookSnapshot(string snapshot);
  }
}