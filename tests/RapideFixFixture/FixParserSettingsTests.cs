using RapideFix;
using Xunit;

namespace RapideFixFixture
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
