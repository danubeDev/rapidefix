using System;
using System.Text;
using RapideFix.Business;
using RapideFix.DataTypes;
using RapideFixFixture.TestTypes;
using Xunit;
using static RapideFix.Business.TagToPropertyMapper;

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
  }
}
