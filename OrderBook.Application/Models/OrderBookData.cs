using System.Text.Json.Serialization;
using System.Text;

namespace OrderBook.Models
{
  public class OrderBookData
  {
    [JsonPropertyName("timestamp")]
    public string Timestamp { get; set; }

    [JsonPropertyName("microtimestamp")]
    public string Microtimestamp { get; set; }

    [JsonPropertyName("bids")]
    public string[][] Bids { get; set; }

    [JsonPropertyName("asks")]
    public string[][] Asks { get; set; }

    public override string ToString()
    {
      var sb = new StringBuilder();
      sb.AppendLine($"Timestamp: {Timestamp}");
      sb.AppendLine($"Microtimestamp: {Microtimestamp}");

      sb.AppendLine("Bids:");
      if (Bids?.Any() == true)
      {
        foreach (var bid in Bids.Take(5)) // Show first 5 bids
        {
          sb.AppendLine($"  Price: {bid[0]}, Amount: {bid[1]}");
        }
      }

      sb.AppendLine("Asks:");
      if (Asks?.Any() == true)
      {
        foreach (var ask in Asks.Take(5)) // Show first 5 asks
        {
          sb.AppendLine($"  Price: {ask[0]}, Amount: {ask[1]}");
        }
      }

      return sb.ToString();
    }
  }
}