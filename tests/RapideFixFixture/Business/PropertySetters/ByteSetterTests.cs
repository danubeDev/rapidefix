using RapideFix.Attributes;
using RapideFix.Business.Data;
using RapideFix.Business.PropertySetters;
using RapideFix.DataTypes;
using Xunit;

namespace RapideFixFixture.Business.PropertySetters
{
  public class ByteSetterTests
  {
    private class MockClass
    {
      [FixTag(1)]
      public byte Tag1 { get; set; }
    }

    private struct MockStruct
    {
      [FixTag(1)]
      public byte Tag1 { get; set; }
    }

    [Fact]
    public void GivenByte_Set_SetsParsedValue()
    {
      var targetObject = new MockClass();
      var uut = new ByteSetter();
      var mappingDetails = new TagMapLeaf(targetObject.GetType().GetProperty(nameof(targetObject.Tag1)), uut);
      var messageContext = new FixMessageContext();
      uut.Set("=", mappingDetails, messageContext, targetObject);
      Assert.Equal(61, targetObject.Tag1);
    }

    [Fact]
    public void GivenByte_SetTarget_SetsParsedValue()
    {
      var targetObject = new MockStruct();
      var uut = new ByteSetter();
      var mappingDetails = new TagMapLeaf(targetObject.GetType().GetProperty(nameof(targetObject.Tag1)), uut);
      var messageContext = new FixMessageContext();
      uut.SetTarget<MockStruct>("=", mappingDetails, messageContext, ref targetObject);
      Assert.Equal(61, targetObject.Tag1);
    }



  }
}
