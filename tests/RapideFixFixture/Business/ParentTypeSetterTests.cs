using System;
using System.Collections.Generic;
using System.Linq;
using RapideFix.Business;
using RapideFix.Business.Data;
using RapideFix.DataTypes;
using RapideFixFixture.TestTypes;
using Xunit;

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
        Parents = new List<TagMapNode>
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
        Parents = new List<TagMapNode>
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

    [Fact]
    public void GivenEnumeratedParents_Set_ReturnsFirstElement()
    {
      var targetObject = new TestTypeParent() { Tag59s = new TestMany[3] };
      var uut = new ParentTypeSetter();
      var valueToSet = new byte[1].AsSpan();
      var mappingDetails = new TagMapLeaf()
      {
        Current = typeof(TestMany).GetProperty("Tag60"),
        Parents = new List<TagMapNode>
        {
          new TagMapNode()
          {
            IsEnumerable = true,
            Current = targetObject.GetType().GetProperty(nameof(targetObject.Tag59s)),
            InnerType = typeof(TestMany)
          }
        }
      };
      var messageContext = new FixMessageContext();
      var result = uut.Set(valueToSet, mappingDetails, messageContext, targetObject);

      Assert.NotNull(targetObject.Tag59s.First());
      Assert.Equal(result, targetObject.Tag59s.First());
    }

    [Fact]
    public void GivenSecondTagOnEnumerated_Set_ReturnsFirstElement()
    {
      var targetObject = new TestTypeParent() { Tag59s = new TestMany[3] };
      var uut = new ParentTypeSetter();
      var valueToSet = new byte[1].AsSpan();
      var mappingTag60 = new TagMapLeaf()
      {
        Current = typeof(TestMany).GetProperty("Tag60"),
        Parents = new List<TagMapNode>
       {
          new TagMapNode()
          {
            IsEnumerable = true,
            Current = targetObject.GetType().GetProperty(nameof(targetObject.Tag59s)),
            InnerType = typeof(TestMany)
          }
        }
      };
      var mappingTag601 = new TagMapLeaf()
      {
        Current = typeof(TestMany).GetProperty("Tag601"),
        Parents = new List<TagMapNode>
        {
          new TagMapNode()
          {
            IsEnumerable = true,
            Current = targetObject.GetType().GetProperty(nameof(targetObject.Tag59s)),
            InnerType = typeof(TestMany)
          }
        }
      };
      var messageContext = new FixMessageContext();
      uut.Set(valueToSet, mappingTag60, messageContext, targetObject);
      var result = uut.Set(valueToSet, mappingTag601, messageContext, targetObject);

      Assert.NotNull(targetObject.Tag59s.First());
      Assert.Equal(result, targetObject.Tag59s.First());
    }

    [Fact]
    public void GivenEnumeratedParents_SetTwice_ReturnsSecondElement()
    {
      var targetObject = new TestTypeParent() { Tag59s = new TestMany[3] };
      var uut = new ParentTypeSetter();
      var valueToSet = new byte[1].AsSpan();
      var mappingDetails = new TagMapLeaf()
      {
        Current = typeof(TestMany).GetProperty("Tag60"),
        Parents = new List<TagMapNode>
        {
          new TagMapNode()
          {
            IsEnumerable = true,
            Current = targetObject.GetType().GetProperty(nameof(targetObject.Tag59s)),
            InnerType = typeof(TestMany)
          }
        }
      };
      var messageContext = new FixMessageContext();
      uut.Set(valueToSet, mappingDetails, messageContext, targetObject);
      var result = uut.Set(valueToSet, mappingDetails, messageContext, targetObject);

      Assert.NotNull(targetObject.Tag59s.First());
      Assert.Equal(result, targetObject.Tag59s.Skip(1).First());
    }


  }
}
