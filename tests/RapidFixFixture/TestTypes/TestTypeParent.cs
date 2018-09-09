using System.Collections.Generic;
using System.ComponentModel;
using RapideFix.Attributes;

namespace RapideFixFixture.TestTypes
{
  [MessageType("A")]
  public class TestTypeParent
  {
    [FixTag(55)]
    public string Tag55 { get; set; }

    [RepeatingGroup(56)]
    [FixTag(57)]
    public IEnumerable<string> Tag57s { get; set; }

    public TestChild CustomType { get; set; }

    [RepeatingGroup(59)]
    public IEnumerable<TestMany> Tag59s { get; set; }

    [FixTag(61)]
    [TypeConverter(typeof(TestConverter))]
    public TestConvertable Tag61 { get; set; }

    [FixTag(62)]
    public int Tag62 { get; set; }

    [FixTag(63)]
    public int Tag63 { get; set; }

    [RepeatingGroup(64)]
    [FixTag(65)]
    [TypeConverter(typeof(TestConverter))]
    public IEnumerable<TestConvertable> Tag65s { get; set; }

    [FixTag(67)]
    public double Tag67 { get; set; }
  }

}
