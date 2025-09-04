using System.Threading.Tasks;
using OrderBook.Models;

namespace OrderBook.Services
{
  public interface IExternalApiService
  {
    Task<OrderBookData> GetOrderBookDataAsync();
  }
}