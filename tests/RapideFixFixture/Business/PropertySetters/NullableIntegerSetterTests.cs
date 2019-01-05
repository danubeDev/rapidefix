using RapideFix.Attributes;
using RapideFix.Business.Data;
using RapideFix.Business.PropertySetters;
using RapideFix.DataTypes;
using Xunit;

namespace RapideFixFixture.Business.PropertySetters
{
  public class NullableIntegerSetterTests
  {
    private class MockClass
    {
      [FixTag(1)]
      public int? Tag1 { get; set; }
    }

    private struct MockStruct
    {
      [FixTag(1)]
      public int? Tag1 { get; set; }
    }

    [Fact]
    public void GivenInteger_Set_SetsParsedValue()
    {
      var targetObject = new MockClass();
      var uut = new NullableIntegerSetter();
      var mappingDetails = new TagMapLeaf() { Current = targetObject.GetType().GetProperty(nameof(targetObject.Tag1)), Setter = uut };
      var messageContext = new FixMessageContext();
      uut.Set(1.ToString(), mappingDetails, messageContext, targetObject);
      Assert.Equal(1, targetObject.Tag1.Value);
    }

    [Fact]
    public void GivenInteger_SetTarget_SetsParsedValue()
    {
      var targetObject = new MockStruct();
      var uut = new NullableIntegerSetter();
      var mappingDetails = new TagMapLeaf() { Current = targetObject.GetType().GetProperty(nameof(targetObject.Tag1)), Setter = uut };
      var messageContext = new FixMessageContext();
      uut.SetTarget<MockStruct>(1.ToString(), mappingDetails, messageContext, ref targetObject);
      Assert.Equal(1, targetObject.Tag1.Value);
    }



  }
}
