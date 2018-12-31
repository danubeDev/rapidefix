using RapideFix.Attributes;

namespace SampleRapideFix.Data
{
  [MessageType("S")]
  public class Slice
  {
    [FixTag(53)]
    public int Quantity { get; set; }

    [FixTag(44)]
    public double Price { get; set; }

    [FixTag(55)]
    public string Symbol { get; set; }
  }
}
