using RapideFix.Business;
using Xunit;

namespace RapideFixFixture.Business
{
  public class SubPropertySetterFactoryTests
  {
    [Fact]
    public void WhenConstruct_DoesNotThrow()
    {
      var ex = Record.Exception(() => new SubPropertySetterFactory());
      Assert.Null(ex);
    }

    [Fact]
    public void When_GetParentTypeSetter_ReturnsNotNull()
    {
      var uut = new SubPropertySetterFactory();
      Assert.NotNull(uut.GetParentPropertySetter());
    }

    [Fact]
    public void When_GetRepeatingGroupTagPropertySetter_ReturnsNotNull()
    {
      var uut = new SubPropertySetterFactory();
      Assert.NotNull(uut.GetRepeatingGroupTagPropertySetter());
    }

    [Fact]
    public void When_GetSimplePropertySetter_ReturnsNotNull()
    {
      var uut = new SubPropertySetterFactory();
      Assert.NotNull(uut.GetSimplePropertySetter());
    }

    [Fact]
    public void When_GetTypeConvertedPropertySetter_ReturnsNotNull()
    {
      var uut = new SubPropertySetterFactory();
      Assert.NotNull(uut.GetTypeConvertedPropertySetter());
    }
  }
}
