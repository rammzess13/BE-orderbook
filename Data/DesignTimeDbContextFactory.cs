using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace OrderBook.Data
{
  public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<OrderBookContext>
  {
    public OrderBookContext CreateDbContext(string[] args)
    {
      IConfigurationRoot configuration = new ConfigurationBuilder()
          .SetBasePath(Directory.GetCurrentDirectory())
          .AddJsonFile("appsettings.json")
          .Build();

      var builder = new DbContextOptionsBuilder<OrderBookContext>();
      var connectionString = configuration.GetConnectionString("DefaultConnection");

      builder.UseSqlite(connectionString);

      return new OrderBookContext(builder.Options);
    }
  }
}