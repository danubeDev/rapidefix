using RapideFix.Attributes;
using System.Collections.Generic;

namespace SampleRapideFix.Data
{
  [MessageType("D")]
  public class Order
  {
    [FixTag(53)]
    public int Quantity { get; set; }

    [FixTag(44)]
    public double Price { get; set; }

    [FixTag(55)]
    public string Symbol { get; set; }

    [FixTag(555)]
    public IEnumerable<Leg> Legs { get; set; }

    [FixTag(1)]
    public string Account { get; set; }
  }
}
