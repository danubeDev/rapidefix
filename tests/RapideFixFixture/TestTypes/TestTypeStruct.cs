using System.Collections.Generic;
using RapideFix.Attributes;

namespace RapideFixFixture.TestTypes
{
  [MessageType("A")]
  public struct TestTypeStruct
  {
    [FixTag(100)]
    public string Tag100 { get; set; }

    [FixTag(101)]
    public int Tag101 { get; set; }

    [FixTag(102)]
    public double Tag102 { get; set; }

    [RepeatingGroup(103)]
    [FixTag(104)]
    public IEnumerable<string> Tag104 { get; set; }
  }
}
