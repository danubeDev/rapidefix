using RapideFix.Attributes;
using RapideFix.Business.Data;
using RapideFix.Business.PropertySetters;
using RapideFix.DataTypes;
using Xunit;

namespace RapideFixFixture.Business.PropertySetters
{
  public class NullableCharSetterTests
  {
    private class MockClass
    {
      [FixTag(1)]
      public char? Tag1 { get; set; }
    }

    private struct MockStruct
    {
      [FixTag(1)]
      public char? Tag1 { get; set; }
    }

    [Fact]
    public void GivenChar_Set_SetsParsedValue()
    {
      var targetObject = new MockClass();
      var uut = new NullableCharSetter();
      var mappingDetails = new TagMapLeaf(targetObject.GetType().GetProperty(nameof(targetObject.Tag1)), uut);
      var messageContext = new FixMessageContext();
      uut.Set("=", mappingDetails, messageContext, targetObject);
      Assert.True(targetObject.Tag1.HasValue);
    }

    [Fact]
    public void GivenChar_SetTarget_SetsParsedValue()
    {
      var targetObject = new MockStruct();
      var uut = new NullableCharSetter();
      var mappingDetails = new TagMapLeaf(targetObject.GetType().GetProperty(nameof(targetObject.Tag1)), uut);
      var messageContext = new FixMessageContext();
      uut.SetTarget<MockStruct>("=", mappingDetails, messageContext, ref targetObject);
      Assert.True(targetObject.Tag1.HasValue);
    }



  }
}
