using System;
using RapideFix.Attributes;
using RapideFix.Business.Data;
using RapideFix.Business.PropertySetters;
using RapideFix.DataTypes;
using Xunit;

namespace RapideFixFixture.Business.PropertySetters
{
  public class NullableDateTimeOffsetSetterTests
  {
    private class MockClass
    {
      [FixTag(1)]
      public DateTimeOffset? Tag1 { get; set; }
    }

    private struct MockStruct
    {
      [FixTag(1)]
      public DateTimeOffset? Tag1 { get; set; }
    }

    [Fact]
    public void GivenDateTimeOffset_Set_SetsParsedValue()
    {
      var targetObject = new MockClass();
      var uut = new NullableDateTimeOffsetSetter();
      var mappingDetails = new TagMapLeaf(targetObject.GetType().GetProperty(nameof(targetObject.Tag1)), uut);
      var messageContext = new FixMessageContext();
      uut.Set("20190104-19:02:04", mappingDetails, messageContext, targetObject);
      Assert.True(targetObject.Tag1.HasValue);
    }

    [Fact]
    public void GivenDateTimeOffset_SetTarget_SetsParsedValue()
    {
      var targetObject = new MockStruct();
      var uut = new NullableDateTimeOffsetSetter();
      var mappingDetails = new TagMapLeaf(targetObject.GetType().GetProperty(nameof(targetObject.Tag1)), uut);
      var messageContext = new FixMessageContext();
      uut.SetTarget<MockStruct>("20190104-19:02:04", mappingDetails, messageContext, ref targetObject);
      Assert.True(targetObject.Tag1.HasValue);
    }



  }
}
