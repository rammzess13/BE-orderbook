using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace OrderBook.Hubs
{
  public class OrderBookHub : Hub
  {
    public override async Task OnConnectedAsync()
    {
      await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
      await base.OnDisconnectedAsync(exception);
    }
  }
}