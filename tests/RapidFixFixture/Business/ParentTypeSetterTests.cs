using System;
using RapideFix.Business;
using RapideFix.DataTypes;
using RapideFixFixture.TestTypes;
using Xunit;
using static RapideFix.Business.TagToPropertyMapper;

namespace RapideFixFixture.Business
{
  public class ParentTypeSetterTests
  {
    [Fact]
    public void GivenParents_Set_CreatesParentTypes()
    {
      var targetObject = new TestTypeParent();
      var uut = new ParentTypeSetter();
      var valueToSet = new byte[1].AsSpan();
      var mappingDetails = new TagMapLeaf()
      {
        Parents = new[]
        {
          new TagMapNode() { Current = targetObject.GetType().GetProperty(nameof(targetObject.CustomType)) }
        }
      };
      var messageContext = new FixMessageContext();
      var result = uut.Set(valueToSet, mappingDetails, messageContext, targetObject);

      Assert.NotNull(targetObject.CustomType);
      Assert.Equal(result, targetObject.CustomType);
    }

    [Fact]
    public void GivenCachedInstance_Set_CreatesParentTypes()
    {
      var targetObject = new TestTypeParent();
      var uut = new ParentTypeSetter();
      var valueToSet = new byte[1].AsSpan();
      var mappingDetails = new TagMapLeaf()
      {
        Parents = new[]
        {
          new TagMapNode() { Current = targetObject.GetType().GetProperty(nameof(targetObject.CustomType)) }
        }
      };
      var messageContext = new FixMessageContext();
      uut.Set(valueToSet, mappingDetails, messageContext, targetObject);
      var result = uut.Set(valueToSet, mappingDetails, messageContext, targetObject);

      Assert.NotNull(targetObject.CustomType);
      Assert.Equal(result, targetObject.CustomType);
    }

  }
}
