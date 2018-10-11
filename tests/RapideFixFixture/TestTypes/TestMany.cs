using RapideFix.Attributes;

namespace RapideFixFixture.TestTypes
{
  public class TestMany
  {
    [FixTag(60)]
    public string Tag60 { get; set; }

    [FixTag(601)]
    public string Tag601 { get; set; }
  }

}