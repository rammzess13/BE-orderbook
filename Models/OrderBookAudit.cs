using System;

namespace OrderBook.Models
{
  public class OrderBookAudit
  {
    public int Id { get; set; }
    public string Timestamp { get; set; }
    public string Microtimestamp { get; set; }
    public string RawData { get; set; }
    public DateTime LoggedAt { get; set; }
  }
}