using RapideFix.Attributes;

namespace SampleRapideFix.Data
{
  [MessageType("S")]
  public class Slice
  {
    [FixTag(153)]
    public int Quantity { get; set; }

    [FixTag(144)]
    public double Price { get; set; }

    [FixTag(155)]
    public string Symbol { get; set; }
  }
}
