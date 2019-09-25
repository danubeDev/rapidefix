using RapideFix.Attributes;
using RapideFix.Business.Data;
using RapideFix.Business.PropertySetters;
using RapideFix.DataTypes;
using Xunit;

namespace RapideFixFixture.Business.PropertySetters
{
  public class DecimalSetterTests
  {
    private class MockClass
    {
      [FixTag(1)]
      public decimal Tag1 { get; set; }
    }

    private struct MockStruct
    {
      [FixTag(1)]
      public decimal Tag1 { get; set; }
    }

    [Fact]
    public void GivenDecimal_Set_SetsParsedValue()
    {
      var targetObject = new MockClass();
      var uut = new DecimalSetter();
      var mappingDetails = new TagMapLeaf(targetObject.GetType().GetProperty(nameof(targetObject.Tag1)), uut);
      var messageContext = new FixMessageContext();
      uut.Set(1M.ToString(), mappingDetails, messageContext, targetObject);
      Assert.Equal(1, targetObject.Tag1);
    }

    [Fact]
    public void GivenDecimal_SetTarget_SetsParsedValue()
    {
      var targetObject = new MockStruct();
      var uut = new DecimalSetter();
      var mappingDetails = new TagMapLeaf(targetObject.GetType().GetProperty(nameof(targetObject.Tag1)), uut);
      var messageContext = new FixMessageContext();
      uut.SetTarget<MockStruct>(1M.ToString(), mappingDetails, messageContext, ref targetObject);
      Assert.Equal(1, targetObject.Tag1);
    }



  }
}
