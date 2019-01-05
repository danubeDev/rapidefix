using RapideFix.Attributes;
using RapideFix.Business.Data;
using RapideFix.Business.PropertySetters;
using RapideFix.DataTypes;
using Xunit;

namespace RapideFixFixture.Business.PropertySetters
{
  public class StringSetterTests
  {
    private class MockClass
    {
      [FixTag(1)]
      public string Tag1 { get; set; }
    }

    private struct MockStruct
    {
      [FixTag(1)]
      public string Tag1 { get; set; }
    }

    [Fact]
    public void GivenChar_Set_SetsParsedValue()
    {
      var targetObject = new MockClass();
      var uut = new StringSetter();
      var mappingDetails = new TagMapLeaf() { Current = targetObject.GetType().GetProperty(nameof(targetObject.Tag1)), Setter = uut };
      var messageContext = new FixMessageContext();
      uut.Set("ab", mappingDetails, messageContext, targetObject);
      Assert.Equal("ab", targetObject.Tag1);
    }

    [Fact]
    public void GivenChar_SetTarget_SetsParsedValue()
    {
      var targetObject = new MockStruct();
      var uut = new StringSetter();
      var mappingDetails = new TagMapLeaf() { Current = targetObject.GetType().GetProperty(nameof(targetObject.Tag1)), Setter = uut };
      var messageContext = new FixMessageContext();
      uut.SetTarget<MockStruct>("ab", mappingDetails, messageContext, ref targetObject);
      Assert.Equal("ab", targetObject.Tag1);
    }



  }
}
