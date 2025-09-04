using System.Text.Json.Serialization;

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

    // public IEnumerable<(decimal Price, decimal Amount)> ParsedBids =>
    //     Bids?.Select(bid => (
    //         decimal.Parse(bid[0], System.Globalization.CultureInfo.InvariantCulture),
    //         decimal.Parse(bid[1], System.Globalization.CultureInfo.InvariantCulture)
    //     )) ?? Enumerable.Empty<(decimal, decimal)>();

    // public IEnumerable<(decimal Price, decimal Amount)> ParsedAsks =>
    //     Asks?.Select(ask => (
    //         decimal.Parse(ask[0], System.Globalization.CultureInfo.InvariantCulture),
    //         decimal.Parse(ask[1], System.Globalization.CultureInfo.InvariantCulture)
    //     )) ?? Enumerable.Empty<(decimal, decimal)>();
  }
}