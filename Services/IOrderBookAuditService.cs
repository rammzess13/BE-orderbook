using System.Threading.Tasks;

namespace OrderBook.Services
{
  public interface IOrderBookAuditService
  {
    Task LogOrderBookSnapshot(string orderBookData);
  }
}