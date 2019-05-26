using RapideFix.Attributes;
using RapideFix.Business.Data;
using RapideFix.Business.PropertySetters;
using RapideFix.DataTypes;
using Xunit;

namespace RapideFixFixture.Business.PropertySetters
{
  public class LongSetterTests
  {
    private class MockClass
    {
      [FixTag(1)]
      public long Tag1 { get; set; }
    }

    private struct MockStruct
    {
      [FixTag(1)]
      public long Tag1 { get; set; }
    }

    [Fact]
    public void GivenLong_Set_SetsParsedValue()
    {
      var targetObject = new MockClass();
      var uut = new LongSetter();
      var mappingDetails = new TagMapLeaf(targetObject.GetType().GetProperty(nameof(targetObject.Tag1)), uut);
      var messageContext = new FixMessageContext();
      uut.Set(1L.ToString(), mappingDetails, messageContext, targetObject);
      Assert.Equal(1L, targetObject.Tag1);
    }

    [Fact]
    public void GivenLong_SetTarget_SetsParsedValue()
    {
      var targetObject = new MockStruct();
      var uut = new LongSetter();
      var mappingDetails = new TagMapLeaf(targetObject.GetType().GetProperty(nameof(targetObject.Tag1)), uut);
      var messageContext = new FixMessageContext();
      uut.SetTarget<MockStruct>(1L.ToString(), mappingDetails, messageContext, ref targetObject);
      Assert.Equal(1L, targetObject.Tag1);
    }



  }
}
