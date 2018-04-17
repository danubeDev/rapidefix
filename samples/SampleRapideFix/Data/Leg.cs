using RapideFix;

namespace SampleRapideFix.Data
{
  public class Leg
  {
    [FixTag(687)]
    public int Quantity { get; set; }

    [FixTag(566)]
    public double Price { get; set; }

    [FixTag(600)]
    public string Symbol { get; set; }
  }
}
