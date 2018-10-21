using RapideFix.Attributes;
using Xunit;

namespace RapideFixFixture.Attributes
{
  public class RepeatingGroupAttributeTests
  {
    [Theory]
    [InlineData(25)]
    [InlineData(478)]
    public void GivenNumber_Construct_DoesNotThrow(int tag)
    {
      var exception = Record.Exception(() => new RepeatingGroupAttribute(tag));
      Assert.Null(exception);

    }
  }
}
