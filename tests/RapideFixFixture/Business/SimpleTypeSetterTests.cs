using System;
using System.Linq;
using System.Text;
using RapideFix.Business;
using RapideFix.Business.Data;
using RapideFix.DataTypes;
using RapideFixFixture.TestTypes;
using Xunit;

namespace RapideFixFixture.Business
{
  public class SimpleTypeSetterTests
  {
    [Fact]
    public void GivenIntegerAndMappingDetails_Set_SetsValueOnTargetObject()
    {
      var targetObject = new TestTypeParent();
      var uut = new SimpleTypeSetter();
      var valueToSet = Encoding.ASCII.GetBytes("12357").AsSpan();
      var mappingDetails = new TagMapLeaf() { Current = targetObject.GetType().GetProperty(nameof(targetObject.Tag62)) };
      var messageContext = new FixMessageContext();
      uut.Set(valueToSet, mappingDetails, messageContext, targetObject);

      Assert.Equal(12357, targetObject.Tag62);
    }

    [Fact]
    public void GivenNullable_Set_SetsValueOnTargetObject()
    {
      var targetObject = new TestTypeParent();
      var uut = new SimpleTypeSetter();
      var valueToSet = Encoding.ASCII.GetBytes("12357").AsSpan();
      var mappingDetails = new TagMapLeaf() { Current = targetObject.GetType().GetProperty(nameof(targetObject.Tag63)) };
      var messageContext = new FixMessageContext();
      uut.Set(valueToSet, mappingDetails, messageContext, targetObject);

      Assert.Equal(12357, targetObject.Tag63);
    }

    [Fact]
    public void GivenEnumerableValue_Set_SetsValueOnTargetObjectsProperIndex()
    {
      var targetObject = new TestTypeParent() { Tag57s = new string[3] };
      var uut = new SimpleTypeSetter();
      var valueToSet = Encoding.ASCII.GetBytes("12357").AsSpan();
      var mappingDetails = new TagMapLeaf()
      {
        IsEnumerable = true,
        Current = targetObject.GetType().GetProperty(nameof(targetObject.Tag57s)),
        RepeatingTagNumber = 56,
        InnerType = typeof(string)
      };
      var messageContext = new FixMessageContext();
      uut.Set(valueToSet, mappingDetails, messageContext, targetObject);

      Assert.Equal("12357", targetObject.Tag57s.First());
    }

    [Fact]
    public void GivenEnumerableAtSecond_Set_SetsValueOnTargetObjectsProperIndex()
    {
      var targetObject = new TestTypeParent() { Tag57s = new string[3] };
      var uut = new SimpleTypeSetter();
      var valueToSet = Encoding.ASCII.GetBytes("12357").AsSpan();
      var mappingDetails = new TagMapLeaf()
      {
        IsEnumerable = true,
        Current = targetObject.GetType().GetProperty(nameof(targetObject.Tag57s)),
        RepeatingTagNumber = 56,
        InnerType = typeof(string)
      };
      var messageContext = new FixMessageContext();
      uut.Set(valueToSet, mappingDetails, messageContext, targetObject);
      // Act: setting the 2nd element
      uut.Set(valueToSet, mappingDetails, messageContext, targetObject);

      Assert.Equal("12357", targetObject.Tag57s.ToArray()[1]);
    }

    [Fact]
    public void GivenStructAndMappingDetails_Set_SetsValueOnTargetObject()
    {
      var targetObject = new TestTypeStruct();
      var uut = new SimpleTypeSetter();
      var integerToSet = Encoding.ASCII.GetBytes("12357").AsSpan();
      var stringToSet = Encoding.ASCII.GetBytes("message").AsSpan();
      var doubleToSet = Encoding.ASCII.GetBytes("123.456").AsSpan();
      var mappingDetailsInt = new TagMapLeaf() { Current = targetObject.GetType().GetProperty(nameof(targetObject.Tag101)) };
      var mappingDetailsString = new TagMapLeaf() { Current = targetObject.GetType().GetProperty(nameof(targetObject.Tag100)) };
      var mappingDetailsDouble = new TagMapLeaf() { Current = targetObject.GetType().GetProperty(nameof(targetObject.Tag102)) };
      var messageContext = new FixMessageContext();
      uut.SetTarget(integerToSet, mappingDetailsInt, messageContext, ref targetObject);
      uut.SetTarget(stringToSet, mappingDetailsString, messageContext, ref targetObject);
      uut.SetTarget(doubleToSet, mappingDetailsDouble, messageContext, ref targetObject);

      Assert.Equal(12357, targetObject.Tag101);
      Assert.Equal("message", targetObject.Tag100);
      Assert.Equal(123.456, targetObject.Tag102);
    }

    [Fact]
    public void GivenStructAndEnumerableMappingDetails_Set_SetsValueOnTargetObject()
    {
      var targetObject = new TestTypeStruct() { Tag104 = new string[2] };
      var uut = new SimpleTypeSetter();
      var valueToSet = Encoding.ASCII.GetBytes("message").AsSpan();
      var mappingDetails = new TagMapLeaf()
      {
        IsEnumerable = true,
        Current = targetObject.GetType().GetProperty(nameof(targetObject.Tag104)),
        RepeatingTagNumber = 103,
        InnerType = typeof(string)
      };
      var messageContext = new FixMessageContext();
      uut.SetTarget(valueToSet, mappingDetails, messageContext, ref targetObject);
      uut.SetTarget(valueToSet, mappingDetails, messageContext, ref targetObject);

      Assert.Equal("message", targetObject.Tag104.First());
      Assert.Equal("message", targetObject.Tag104.Last());
    }
  }
}
