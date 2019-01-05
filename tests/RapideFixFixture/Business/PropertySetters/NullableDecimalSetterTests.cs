using RapideFix.Attributes;
using RapideFix.Business.Data;
using RapideFix.Business.PropertySetters;
using RapideFix.DataTypes;
using Xunit;

namespace RapideFixFixture.Business.PropertySetters
{
  public class NullableDecimalSetterTests
  {
    private class MockClass
    {
      [FixTag(1)]
      public decimal? Tag1 { get; set; }
    }

    private struct MockStruct
    {
      [FixTag(1)]
      public decimal? Tag1 { get; set; }
    }

    [Fact]
    public void GivenDecimal_Set_SetsParsedValue()
    {
      var targetObject = new MockClass();
      var uut = new NullableDecimalSetter();
      var mappingDetails = new TagMapLeaf() { Current = targetObject.GetType().GetProperty(nameof(targetObject.Tag1)), Setter = uut };
      var messageContext = new FixMessageContext();
      uut.Set(1M.ToString(), mappingDetails, messageContext, targetObject);
      Assert.True(targetObject.Tag1.HasValue);
    }

    [Fact]
    public void GivenDecimal_SetTarget_SetsParsedValue()
    {
      var targetObject = new MockStruct();
      var uut = new NullableDecimalSetter();
      var mappingDetails = new TagMapLeaf() { Current = targetObject.GetType().GetProperty(nameof(targetObject.Tag1)), Setter = uut };
      var messageContext = new FixMessageContext();
      uut.SetTarget<MockStruct>(1M.ToString(), mappingDetails, messageContext, ref targetObject);
      Assert.True(targetObject.Tag1.HasValue);
    }



  }
}
