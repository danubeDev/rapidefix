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
      var mappingDetails = new EnumerableTagMapLeaf()
      {
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
      var mappingDetails = new EnumerableTagMapLeaf()
      {
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
  }
}
