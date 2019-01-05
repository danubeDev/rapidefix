using System;
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
      Assert.NotNull(uut.GetParentSetter(null));
    }

    [Fact]
    public void When_GetRepeatingGroupTagPropertySetter_ReturnsNotNull()
    {
      var uut = new SubPropertySetterFactory();
      Assert.NotNull(uut.GetRepeatingGroupTagSetter(null));
    }

    [Fact]
    public void When_GetSimplePropertySetter_ReturnsNotNull()
    {
      var uut = new SubPropertySetterFactory();
      Assert.NotNull(uut.GetSetter(null, typeof(string)));
      Assert.NotNull(uut.GetSetter(null, typeof(bool)));
      Assert.NotNull(uut.GetSetter(null, typeof(byte)));
      Assert.NotNull(uut.GetSetter(null, typeof(char)));
      Assert.NotNull(uut.GetSetter(null, typeof(DateTimeOffset)));
      Assert.NotNull(uut.GetSetter(null, typeof(decimal)));
      Assert.NotNull(uut.GetSetter(null, typeof(double)));
      Assert.NotNull(uut.GetSetter(null, typeof(float)));
      Assert.NotNull(uut.GetSetter(null, typeof(int)));
      Assert.NotNull(uut.GetSetter(null, typeof(long)));
      Assert.NotNull(uut.GetSetter(null, typeof(short)));
      Assert.NotNull(uut.GetSetter(null, typeof(bool?)));
      Assert.NotNull(uut.GetSetter(null, typeof(byte?)));
      Assert.NotNull(uut.GetSetter(null, typeof(char?)));
      Assert.NotNull(uut.GetSetter(null, typeof(DateTimeOffset?)));
      Assert.NotNull(uut.GetSetter(null, typeof(decimal?)));
      Assert.NotNull(uut.GetSetter(null, typeof(double?)));
      Assert.NotNull(uut.GetSetter(null, typeof(float?)));
      Assert.NotNull(uut.GetSetter(null, typeof(int?)));
      Assert.NotNull(uut.GetSetter(null, typeof(long?)));
      Assert.NotNull(uut.GetSetter(null, typeof(short?)));
    }

    [Fact]
    public void When_GetTypeConvertedPropertySetter_ReturnsNotNull()
    {
      var uut = new SubPropertySetterFactory();
      Assert.NotNull(uut.GetTypeConvertingSetter(null));
    }
  }
}
