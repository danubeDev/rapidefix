using RapideFix.Attributes;
using RapideFix.Business.Data;
using RapideFix.Business.PropertySetters;
using RapideFix.DataTypes;
using Xunit;

namespace RapideFixFixture.Business.PropertySetters
{
  public class NullableShortSetterTests
  {
    private class MockClass
    {
      [FixTag(1)]
      public short? Tag1 { get; set; }
    }

    private struct MockStruct
    {
      [FixTag(1)]
      public short? Tag1 { get; set; }
    }

    [Fact]
    public void GivenShort_Set_SetsParsedValue()
    {
      var targetObject = new MockClass();
      var uut = new NullableShortSetter();
      var mappingDetails = new TagMapLeaf(targetObject.GetType().GetProperty(nameof(targetObject.Tag1)), uut);
      var messageContext = new FixMessageContext();
      uut.Set(((short)1).ToString(), mappingDetails, messageContext, targetObject);
      Assert.Equal(1, targetObject.Tag1.Value);
    }

    [Fact]
    public void GivenShort_SetTarget_SetsParsedValue()
    {
      var targetObject = new MockStruct();
      var uut = new NullableShortSetter();
      var mappingDetails = new TagMapLeaf(targetObject.GetType().GetProperty(nameof(targetObject.Tag1)), uut);
      var messageContext = new FixMessageContext();
      uut.SetTarget<MockStruct>(((short)1).ToString(), mappingDetails, messageContext, ref targetObject);
      Assert.Equal(1, targetObject.Tag1.Value);
    }



  }
}
