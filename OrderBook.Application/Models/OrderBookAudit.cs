using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrderBook.Models
{
  public class OrderBookAudit
  {
    [Key]
    public int Id { get; set; }

    [Required]
    public string Timestamp { get; set; }

    [Required]
    public string Microtimestamp { get; set; }

    [Required]
    [Column(TypeName = "TEXT")]
    public string RawData { get; set; }

    [Required]
    public DateTime LoggedAt { get; set; }
  }
}