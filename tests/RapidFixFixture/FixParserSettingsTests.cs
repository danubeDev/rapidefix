using RapideFix;
using Xunit;

namespace RapidFixFixture
{
  public class FixParserSettingsTests
  {
    [Fact]
    public void Test()
    {
      var exception = Record.Exception(() => new FixParserSettings());
      Assert.Null(exception);
    }
  }
}
