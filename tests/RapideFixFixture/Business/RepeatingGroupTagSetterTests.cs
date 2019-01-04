using System;
using System.Linq;
using System.Reflection;
using RapideFix;
using RapideFix.Business;
using RapideFix.Business.Data;
using RapideFix.DataTypes;
using RapideFixFixture.TestTypes;
using Xunit;

namespace RapideFixFixture.Business
{
  public class RepeatingGroupTagSetterTests
  {
    [Fact]
    public void GivenIEnumerableT_Set_CreatesArryOfT()
    {
      var targetObject = new TestTypeParent();
      var uut = new RepeatingGroupTagSetter();
      var valueToSet = new byte[2];
      IntegerToFixConverter.Instance.Convert(15, valueToSet, 2);
      PropertyInfo propertyInfo = targetObject.GetType().GetProperty(nameof(targetObject.Tag59s));
      var mappingDetails = TagMapLeaf.CreateRepeatingTag<TagMapLeaf>(propertyInfo, propertyInfo.PropertyType.GetGenericArguments()[0], uut);
      var messageContext = new FixMessageContext();
      var result = uut.Set(valueToSet, mappingDetails, messageContext, targetObject);

      Assert.NotNull(targetObject.Tag59s);
      Assert.Equal(15, targetObject.Tag59s.Count());
    }

    [Fact]
    public void GivenIEnumerablePrimitive_Set_CreatesArryOfPrimitive()
    {
      var targetObject = new TestTypeParent();
      var uut = new RepeatingGroupTagSetter();
      var valueToSet = new byte[2];
      IntegerToFixConverter.Instance.Convert(15, valueToSet, 2);
      PropertyInfo propertyInfo = targetObject.GetType().GetProperty(nameof(targetObject.Tag57s));
      var mappingDetails = TagMapLeaf.CreateRepeatingTag<TagMapLeaf>(propertyInfo, propertyInfo.PropertyType.GetGenericArguments()[0], uut);
      var messageContext = new FixMessageContext();
      var result = uut.Set(valueToSet, mappingDetails, messageContext, targetObject);

      Assert.NotNull(targetObject.Tag57s);
      Assert.Equal(15, targetObject.Tag57s.Count());
    }

    [Fact]
    public void GivenCachedSettersIEnumerablePrimitive_Set_DoesNotSetAgain()
    {
      var targetObject = new TestTypeParent();
      var uut = new RepeatingGroupTagSetter();
      var valueToSet = new byte[2];
      IntegerToFixConverter.Instance.Convert(15, valueToSet, 2);
      PropertyInfo propertyInfo = targetObject.GetType().GetProperty(nameof(targetObject.Tag57s));
      var mappingDetails = TagMapLeaf.CreateRepeatingTag<TagMapLeaf>(propertyInfo, propertyInfo.PropertyType.GetGenericArguments()[0], uut);
      var messageContext = new FixMessageContext();
      uut.Set(valueToSet, mappingDetails, messageContext, targetObject);

      // Try to set a different value
      IntegerToFixConverter.Instance.Convert(16, valueToSet, 2);
      var result = uut.Set(valueToSet, mappingDetails, messageContext, targetObject);

      Assert.NotNull(targetObject.Tag57s);
      Assert.Equal(15, targetObject.Tag57s.Count());
    }

    [Fact]
    public void GivenIEnumerableT_SetAsString_CreatesArryOfT()
    {
      var targetObject = new TestTypeParent();
      var uut = new RepeatingGroupTagSetter();
      var valueToSet = "4".AsSpan();
      PropertyInfo propertyInfo = targetObject.GetType().GetProperty(nameof(targetObject.Tag59s));
      var mappingDetails = TagMapLeaf.CreateRepeatingTag<TagMapLeaf>(propertyInfo, propertyInfo.PropertyType.GetGenericArguments()[0], uut);
      var messageContext = new FixMessageContext();
      var result = uut.Set(valueToSet, mappingDetails, messageContext, targetObject);

      Assert.NotNull(targetObject.Tag59s);
      Assert.Equal(4, targetObject.Tag59s.Count());
    }

    [Fact]
    public void GivenIEnumerablePrimitive_SetAsString_CreatesArryOfPrimitive()
    {
      var targetObject = new TestTypeParent();
      var uut = new RepeatingGroupTagSetter();
      var valueToSet = "3".AsSpan();
      PropertyInfo propertyInfo = targetObject.GetType().GetProperty(nameof(targetObject.Tag57s));
      var mappingDetails = TagMapLeaf.CreateRepeatingTag<TagMapLeaf>(propertyInfo, propertyInfo.PropertyType.GetGenericArguments()[0], uut);
      var messageContext = new FixMessageContext();
      var result = uut.Set(valueToSet, mappingDetails, messageContext, targetObject);

      Assert.NotNull(targetObject.Tag57s);
      Assert.Equal(3, targetObject.Tag57s.Count());
    }
  }
}
