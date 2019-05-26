using RapideFix.Business.Data;
using RapideFix.Business.PropertySetters;
using RapideFix.DataTypes;
using RapideFixFixture.TestTypes;
using Xunit;

namespace RapideFixFixture.Business.PropertySetters
{
  public class BooleanSetterTests
  {
    [Theory]
    [InlineData("1")]
    [InlineData("Y")]
    [InlineData("True")]
    public void GivenTrueValues_Set_SetsParsedValue(string value)
    {
      var targetObject = new TestTypeParent();
      var uut = new BooleanSetter();
      var mappingDetails = new TagMapLeaf(targetObject.GetType().GetProperty(nameof(targetObject.Tag68)), uut);
      var messageContext = new FixMessageContext();
      uut.Set(value, mappingDetails, messageContext, targetObject);
      Assert.True(targetObject.Tag68);
    }

    [Theory]
    [InlineData("0")]
    [InlineData("N")]
    [InlineData("False")]
    public void GivenFalseValues_Set_SetsParsedValue(string value)
    {
      var targetObject = new TestTypeParent();
      var uut = new BooleanSetter();
      var mappingDetails = new TagMapLeaf(targetObject.GetType().GetProperty(nameof(targetObject.Tag68)), uut);
      var messageContext = new FixMessageContext();
      uut.Set(value, mappingDetails, messageContext, targetObject);
      Assert.False(targetObject.Tag68);
    }


  }
}
