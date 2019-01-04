using RapideFix.Attributes;
using RapideFix.Business.Data;
using RapideFix.Business.PropertySetters;
using RapideFix.DataTypes;
using Xunit;

namespace RapideFixFixture.Business.PropertySetters
{
  public class FloatSetterTests
  {
    private class MockClass
    {
      [FixTag(1)]
      public float Tag1 { get; set; }
    }

    private struct MockStruct
    {
      [FixTag(1)]
      public float Tag1 { get; set; }
    }

    [Fact]
    public void GivenFloat_Set_SetsParsedValue()
    {
      var targetObject = new MockClass();
      var uut = new FloatSetter();
      var mappingDetails = new TagMapLeaf() { Current = targetObject.GetType().GetProperty(nameof(targetObject.Tag1)), Setter = uut };
      var messageContext = new FixMessageContext();
      uut.Set(1.5f.ToString(), mappingDetails, messageContext, targetObject);
      Assert.Equal(1.5, targetObject.Tag1);
    }

    [Fact]
    public void GivenFloat_SetTarget_SetsParsedValue()
    {
      var targetObject = new MockStruct();
      var uut = new FloatSetter();
      var mappingDetails = new TagMapLeaf() { Current = targetObject.GetType().GetProperty(nameof(targetObject.Tag1)), Setter = uut };
      var messageContext = new FixMessageContext();
      uut.SetTarget<MockStruct>(1.5f.ToString(), mappingDetails, messageContext, ref targetObject);
      Assert.Equal(1.5, targetObject.Tag1);
    }



  }
}
