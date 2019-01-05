using System;
using System.Linq;
using System.Text;
using RapideFix.Business;
using RapideFix.Business.Data;
using RapideFix.Business.PropertySetters;
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
      var mappingDetails = new TagMapLeaf() { Current = targetObject.GetType().GetProperty(nameof(targetObject.Tag62)), Setter = new IntegerSetter() };
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
      var mappingDetails = new TagMapLeaf() { Current = targetObject.GetType().GetProperty(nameof(targetObject.Tag63)), Setter = new IntegerSetter() };
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
        InnerType = typeof(string),
        Setter = new StringSetter()
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
        InnerType = typeof(string),
        Setter = new StringSetter()
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
      var mappingDetailsInt = new TagMapLeaf() { Current = targetObject.GetType().GetProperty(nameof(targetObject.Tag101)), Setter = new IntegerSetter() };
      var mappingDetailsString = new TagMapLeaf() { Current = targetObject.GetType().GetProperty(nameof(targetObject.Tag100)), Setter = new StringSetter() };
      var mappingDetailsDouble = new TagMapLeaf() { Current = targetObject.GetType().GetProperty(nameof(targetObject.Tag102)), Setter = new DoubleSetter() };
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
        Setter = new StringSetter(),
        RepeatingTagNumber = 103,
        InnerType = typeof(string)
      };
      var messageContext = new FixMessageContext();
      uut.SetTarget(valueToSet, mappingDetails, messageContext, ref targetObject);
      uut.SetTarget(valueToSet, mappingDetails, messageContext, ref targetObject);

      Assert.Equal("message", targetObject.Tag104.First());
      Assert.Equal("message", targetObject.Tag104.Last());
    }

    [Fact]
    public void GivenIntegerAsString_Set_SetsValueOnTargetObject()
    {
      var targetObject = new TestTypeParent();
      var uut = new SimpleTypeSetter();
      var valueToSet = "12357".AsSpan();
      var mappingDetails = new TagMapLeaf() { Current = targetObject.GetType().GetProperty(nameof(targetObject.Tag62)), Setter = new IntegerSetter() };
      var messageContext = new FixMessageContext();
      uut.Set(valueToSet, mappingDetails, messageContext, targetObject);

      Assert.Equal(12357, targetObject.Tag62);
    }

    [Fact]
    public void GivenNullableIntegerAsString_Set_SetsValueOnTargetObject()
    {
      var targetObject = new TestTypeParent();
      var uut = new SimpleTypeSetter();
      ReadOnlySpan<char> valueToSet = "12357".AsSpan();
      var mappingDetails = new TagMapLeaf() { Current = targetObject.GetType().GetProperty(nameof(targetObject.Tag63)), Setter = new IntegerSetter() };
      var messageContext = new FixMessageContext();
      uut.Set(valueToSet, mappingDetails, messageContext, targetObject);

      Assert.Equal(12357, targetObject.Tag63);
    }

    [Fact]
    public void GivenEnumerableAsString_Set_SetsValueOnTargetObjectsProperIndex()
    {
      var targetObject = new TestTypeParent() { Tag57s = new string[3] };
      var uut = new SimpleTypeSetter();
      var firstValueToSet = "12357";
      var secondValueToSet = "12358";
      var mappingDetails = new TagMapLeaf()
      {
        IsEnumerable = true,
        Current = targetObject.GetType().GetProperty(nameof(targetObject.Tag57s)),
        RepeatingTagNumber = 56,
        InnerType = typeof(string),
        Setter = new StringSetter()
      };
      var messageContext = new FixMessageContext();
      // Setting the 1st element
      uut.Set(firstValueToSet.AsSpan(), mappingDetails, messageContext, targetObject);
      // Setting the 2nd element
      uut.Set(secondValueToSet.AsSpan(), mappingDetails, messageContext, targetObject);

      Assert.Equal(firstValueToSet, targetObject.Tag57s.First());
      Assert.Equal(secondValueToSet, targetObject.Tag57s.Skip(1).First());
    }

    [Fact]
    public void GivenStructAsSting_Set_SetsValueOnTargetObject()
    {
      var targetObject = new TestTypeStruct();
      var uut = new SimpleTypeSetter();
      var integerToSet = "12357".AsSpan();
      var stringToSet = "message".AsSpan();
      var doubleToSet = "123.456".AsSpan();
      var mappingDetailsInt = new TagMapLeaf() { Current = targetObject.GetType().GetProperty(nameof(targetObject.Tag101)), Setter = new IntegerSetter() };
      var mappingDetailsString = new TagMapLeaf() { Current = targetObject.GetType().GetProperty(nameof(targetObject.Tag100)), Setter = new StringSetter() };
      var mappingDetailsDouble = new TagMapLeaf() { Current = targetObject.GetType().GetProperty(nameof(targetObject.Tag102)), Setter = new DoubleSetter() };
      var messageContext = new FixMessageContext();
      uut.SetTarget(integerToSet, mappingDetailsInt, messageContext, ref targetObject);
      uut.SetTarget(stringToSet, mappingDetailsString, messageContext, ref targetObject);
      uut.SetTarget(doubleToSet, mappingDetailsDouble, messageContext, ref targetObject);

      Assert.Equal(12357, targetObject.Tag101);
      Assert.Equal("message", targetObject.Tag100);
      Assert.Equal(123.456, targetObject.Tag102);
    }

    [Fact]
    public void GivenStructAndEnumerableAsString_Set_SetsValueOnTargetObject()
    {
      var targetObject = new TestTypeStruct() { Tag104 = new string[2] };
      var uut = new SimpleTypeSetter();
      var valueToSet = "message";
      var mappingDetails = new TagMapLeaf()
      {
        IsEnumerable = true,
        Current = targetObject.GetType().GetProperty(nameof(targetObject.Tag104)),
        Setter = new StringSetter(),
        RepeatingTagNumber = 103,
        InnerType = typeof(string)
      };
      var messageContext = new FixMessageContext();
      uut.SetTarget(valueToSet.AsSpan(), mappingDetails, messageContext, ref targetObject);
      uut.SetTarget(valueToSet.AsSpan(), mappingDetails, messageContext, ref targetObject);

      Assert.Equal(valueToSet, targetObject.Tag104.First());
      Assert.Equal(valueToSet, targetObject.Tag104.Last());
    }

    [Fact]
    public void GivenBool_Set_SetsValueOnTargetObject()
    {
      var targetObject = new TestTypeParent();
      var uut = new SimpleTypeSetter();
      var valueToSet = Encoding.ASCII.GetBytes("Y").AsSpan();
      var mappingDetails = new TagMapLeaf() { Current = targetObject.GetType().GetProperty(nameof(targetObject.Tag68)), Setter = new BooleanSetter() };
      var messageContext = new FixMessageContext();
      uut.Set(valueToSet, mappingDetails, messageContext, targetObject);
      Assert.True(targetObject.Tag68);
    }

    [Fact]
    public void GivenNullableBool_Set_SetsValueOnTargetObject()
    {
      var targetObject = new TestTypeParent();
      var uut = new SimpleTypeSetter();
      var valueToSet = Encoding.ASCII.GetBytes("Y").AsSpan();
      var mappingDetails = new TagMapLeaf() { Current = targetObject.GetType().GetProperty(nameof(targetObject.Tag69)), Setter = new NullableBooleanSetter() };
      var messageContext = new FixMessageContext();
      uut.Set(valueToSet, mappingDetails, messageContext, targetObject);
      Assert.True(targetObject.Tag69);
      Assert.False(targetObject.Tag68);
    }

    [Fact]
    public void GivenBoolAsString_Set_SetsValueOnTargetObject()
    {
      var targetObject = new TestTypeParent();
      var uut = new SimpleTypeSetter();
      var valueToSet = "Y";
      var mappingDetails = new TagMapLeaf() { Current = targetObject.GetType().GetProperty(nameof(targetObject.Tag68)), Setter = new BooleanSetter() };
      var messageContext = new FixMessageContext();
      uut.Set(valueToSet.AsSpan(), mappingDetails, messageContext, targetObject);
      Assert.True(targetObject.Tag68);
    }

    [Fact]
    public void GivenNullableBoolAsString_Set_SetsValueOnTargetObject()
    {
      var targetObject = new TestTypeParent();
      var uut = new SimpleTypeSetter();
      var valueToSet = "Y";
      var mappingDetails = new TagMapLeaf() { Current = targetObject.GetType().GetProperty(nameof(targetObject.Tag69)), Setter = new NullableBooleanSetter() };
      var messageContext = new FixMessageContext();
      uut.Set(valueToSet.AsSpan(), mappingDetails, messageContext, targetObject);
      Assert.True(targetObject.Tag69);
      Assert.False(targetObject.Tag68);
    }

    [Fact]
    public void GivenDateTimeOffsetAsString_Set_SetsValueOnTargetObject()
    {
      var targetObject = new TestTypeParent();
      var uut = new SimpleTypeSetter();
      var valueToSet = "20181219-18:14:23";
      var mappingDetails = new TagMapLeaf() { Current = targetObject.GetType().GetProperty(nameof(targetObject.Tag70)), Setter = new DateTimeOffsetSetter() };
      var messageContext = new FixMessageContext();
      uut.Set(valueToSet.AsSpan(), mappingDetails, messageContext, targetObject);
      Assert.Equal(new DateTimeOffset(2018, 12, 19, 18, 14, 23, 0, TimeSpan.Zero), targetObject.Tag70);
    }

    [Fact]
    public void GivenNullableDateTimeOffsetAsString_Set_SetsValueOnTargetObject()
    {
      var targetObject = new TestTypeParent();
      var uut = new SimpleTypeSetter();
      var valueToSet = "20181219-18:14:23";
      var mappingDetails = new TagMapLeaf() { Current = targetObject.GetType().GetProperty(nameof(targetObject.Tag71)), Setter = new NullableDateTimeOffsetSetter() };
      var messageContext = new FixMessageContext();
      uut.Set(valueToSet.AsSpan(), mappingDetails, messageContext, targetObject);
      Assert.Equal(new DateTimeOffset(2018, 12, 19, 18, 14, 23, 0, TimeSpan.Zero), targetObject.Tag71.Value);
    }
  }
}
