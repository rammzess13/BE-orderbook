using Microsoft.Extensions.DependencyInjection;

namespace OrderBook.Services
{
  public interface IOrderBookAuditServiceFactory
  {
    IOrderBookAuditService CreateScope();
  }

  public class OrderBookAuditServiceFactory : IOrderBookAuditServiceFactory
  {
    private readonly IServiceScopeFactory _scopeFactory;

    public OrderBookAuditServiceFactory(IServiceScopeFactory scopeFactory)
    {
      _scopeFactory = scopeFactory;
    }

    public IOrderBookAuditService CreateScope()
    {
      var scope = _scopeFactory.CreateScope();
      return scope.ServiceProvider.GetRequiredService<IOrderBookAuditService>();
    }
  }
}