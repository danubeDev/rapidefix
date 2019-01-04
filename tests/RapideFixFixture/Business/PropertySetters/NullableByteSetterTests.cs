using RapideFix.Attributes;
using RapideFix.Business.Data;
using RapideFix.Business.PropertySetters;
using RapideFix.DataTypes;
using RapideFixFixture.TestTypes;
using Xunit;

namespace RapideFixFixture.Business.PropertySetters
{
  public class NullableByteSetterTests
  {
    private class MockClass
    {
      [FixTag(1)]
      public byte? Tag1 { get; set; }
    }

    private struct MockStruct
    {
      [FixTag(1)]
      public byte? Tag1 { get; set; }
    }

    [Fact]
    public void GivenByte_Set_SetsParsedValue()
    {
      var targetObject = new MockClass();
      var uut = new NullableByteSetter();
      var mappingDetails = new TagMapLeaf() { Current = targetObject.GetType().GetProperty(nameof(targetObject.Tag1)), Setter = uut };
      var messageContext = new FixMessageContext();
      uut.Set("=", mappingDetails, messageContext, targetObject);
      Assert.True(targetObject.Tag1.HasValue);
    }

    [Fact]
    public void GivenByte_SetTarget_SetsParsedValue()
    {
      var targetObject = new MockStruct();
      var uut = new NullableByteSetter();
      var mappingDetails = new TagMapLeaf() { Current = targetObject.GetType().GetProperty(nameof(targetObject.Tag1)), Setter = uut };
      var messageContext = new FixMessageContext();
      uut.SetTarget<MockStruct>("=", mappingDetails, messageContext, ref targetObject);
      Assert.True(targetObject.Tag1.HasValue);
    }



  }
}
