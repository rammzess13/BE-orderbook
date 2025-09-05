using Microsoft.EntityFrameworkCore;
using OrderBook.Models;

namespace OrderBook.Data
{
  public class OrderBookContext : DbContext
  {
    public OrderBookContext(DbContextOptions<OrderBookContext> options)
        : base(options)
    {
    }

    public DbSet<OrderBookAudit> OrderBookAudits { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      base.OnModelCreating(modelBuilder);

      modelBuilder.Entity<OrderBookAudit>(entity =>
      {
        entity.HasKey(e => e.Id);
        entity.Property(e => e.LoggedAt)
                  .IsRequired()
                  .HasDefaultValueSql("CURRENT_TIMESTAMP");
        entity.Property(e => e.RawData)
                  .IsRequired()
                  .HasColumnType("TEXT");
      });
    }
  }
}