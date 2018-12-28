using System;
using RapideFix.Attributes;

namespace SampleRapideFix.Data
{
  public struct Quote
  {
    [FixTag(134)]
    public int Quantity { get; set; }

    [FixTag(132)]
    public double Price { get; set; }

    [FixTag(62)]
    public DateTimeOffset Expiry { get; set; }
  }
}
