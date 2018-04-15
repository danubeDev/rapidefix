using RapideFix;
using Xunit;

namespace RapidFixFixture
{
  public class LibTests
  {
    [Fact]
    public void Test()
    {
      var uut = new Lib();
      Assert.NotNull(uut.Hello());
    }
  }
}
