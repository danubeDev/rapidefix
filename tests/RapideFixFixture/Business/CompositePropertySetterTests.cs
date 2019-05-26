using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using RapideFix.Business;
using RapideFix.Business.Data;
using RapideFix.Business.PropertySetters;
using RapideFix.DataTypes;
using RapideFixFixture.TestTypes;
using Xunit;

namespace RapideFixFixture.Business
{
  public class CompositePropertySetterTests
  {
    [Fact]
    public void GivenParent_Set_CallsParentsSetter()
    {
      var testValue = new byte[0];
      var mockParentSetter = new MockPropertySetter();
      var mockSimpleTypeSetter = new MockPropertySetter();

      var uut = new CompositePropertySetter();
      uut.Set(testValue, new TagMapLeaf(MockPropertyInfo.Get, mockSimpleTypeSetter) { Parents = new List<TagMapParent>() { new TagMapParent(MockPropertyInfo.Get, mockParentSetter) } }, new FixMessageContext(), new object());

      Assert.True(mockParentSetter.IsVerified);
      Assert.True(mockSimpleTypeSetter.IsVerified);
    }

    [Fact]
    public void GivenSimpleValue_Set_CallsSimpleSetter()
    {
      var testValue = new byte[0];
      var mockSimpleTypeSetter = new MockPropertySetter();

      var uut = new CompositePropertySetter();
      uut.Set(testValue, new TagMapLeaf(MockPropertyInfo.Get, mockSimpleTypeSetter), new FixMessageContext(), new object());

      Assert.True(mockSimpleTypeSetter.IsVerified);
    }

    [Fact]
    public void GivenRepeatingGroup_Set_CallsRepeatingSetter()
    {
      var testValue = new byte[0];
      var mockSetter = new MockPropertySetter();

      var uut = new CompositePropertySetter();
      uut.Set(testValue, TagMapLeaf.CreateRepeatingTag(null, null, mockSetter), new FixMessageContext(), new object());

      Assert.True(mockSetter.IsVerified);
    }

    [Fact]
    public void GivenTypeConvertedSetter_Set_CallsConvertingSetter()
    {
      var testValue = new byte[0];
      var mockSetter = new MockPropertySetter();

      var uut = new CompositePropertySetter();
      uut.Set(testValue, new TagMapLeaf(MockPropertyInfo.Get, mockSetter) { TypeConverterName = "name" }, new FixMessageContext(), new object());

      Assert.True(mockSetter.IsVerified);
    }

    [Fact]
    public void GivenTypedSimpleValue_SetTarget_CallsTypedSetter()
    {
      var testValue = new byte[0];
      var mockSimpleTypeSetter = new MockPropertySetter();

      var uut = new CompositePropertySetter();
      var targetObject = new object();
      uut.SetTarget(testValue, new TagMapLeaf(MockPropertyInfo.Get, mockSimpleTypeSetter), new FixMessageContext(), ref targetObject);

      Assert.True(mockSimpleTypeSetter.IsVerified);
    }

    [Fact]
    public void GivenParentAsString_Set_CallsParentsSetter()
    {
      var testValue = "test".AsSpan();
      var mockParentSetter = new MockPropertySetter();
      var mockSimpleTypeSetter = new MockPropertySetter();

      var uut = new CompositePropertySetter();
      uut.Set(testValue, new TagMapLeaf(MockPropertyInfo.Get, mockSimpleTypeSetter) { Parents = new List<TagMapParent>() { new TagMapParent(MockPropertyInfo.Get, mockParentSetter) } }, new FixMessageContext(), new object());

      Assert.True(mockParentSetter.IsVerified);
      Assert.True(mockSimpleTypeSetter.IsVerified);
    }

    [Fact]
    public void GivenSimpleValueAsString_Set_CallsSimpleSetter()
    {
      var testValue = "test".AsSpan();
      var mockSimpleTypeSetter = new MockPropertySetter();

      var uut = new CompositePropertySetter();
      uut.Set(testValue, new TagMapLeaf(MockPropertyInfo.Get, mockSimpleTypeSetter), new FixMessageContext(), new object());

      Assert.True(mockSimpleTypeSetter.IsVerified);
    }

    [Fact]
    public void GivenRepeatingGroupAsString_Set_CallsRepeatingSetter()
    {
      var testValue = "test".AsSpan();
      var mockSetter = new MockPropertySetter();

      var uut = new CompositePropertySetter();
      uut.Set(testValue, TagMapLeaf.CreateRepeatingTag(null, null, mockSetter), new FixMessageContext(), new object());

      Assert.True(mockSetter.IsVerified);
    }

    [Fact]
    public void GivenTypeConvertedSetterAsString_Set_CallsConvertingSetter()
    {
      var testValue = "test".AsSpan();
      var mockSetter = new MockPropertySetter();

      var uut = new CompositePropertySetter();
      uut.Set(testValue, new TagMapLeaf(MockPropertyInfo.Get, mockSetter) { TypeConverterName = "name" }, new FixMessageContext(), new object());

      Assert.True(mockSetter.IsVerified);
    }

    [Fact]
    public void GivenTypedSimpleValueAsString_SetTarget_CallsTypedSetter()
    {
      var testValue = "test".AsSpan();
      var mockSetter = new MockPropertySetter();

      var uut = new CompositePropertySetter();
      var targetObject = new object();
      uut.SetTarget(testValue, new TagMapLeaf(MockPropertyInfo.Get, mockSetter), new FixMessageContext(), ref targetObject);

      Assert.True(mockSetter.IsVerified);
    }

    [Fact]
    public void GivenParents_Set_CreatesParentTypes()
    {
      var targetObject = new TestTypeParent();
      var uut = new CompositePropertySetter();
      var valueToSet = new byte[1].AsSpan();
      var mappingDetails = new TagMapLeaf(MockPropertyInfo.Get, new MockPropertySetter())
      {
        Parents = new List<TagMapParent>
        {
          new TagMapParent(targetObject.GetType().GetProperty(nameof(targetObject.CustomType)), new ParentPropertySetter())
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
      var uut = new CompositePropertySetter();
      var valueToSet = new byte[1].AsSpan();
      var mappingDetails = new TagMapLeaf(MockPropertyInfo.Get, new MockPropertySetter())
      {
        Parents = new List<TagMapParent>
        {
          new TagMapParent(targetObject.GetType().GetProperty(nameof(targetObject.CustomType)), new ParentPropertySetter())
        },
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
      var uut = new CompositePropertySetter();
      var valueToSet = new byte[1].AsSpan();
      var mappingDetails = new TagMapLeaf(typeof(TestMany).GetProperty("Tag60"), new MockPropertySetter())
      {
        Parents = new List<TagMapParent>
        {
          new TagMapParent(targetObject.GetType().GetProperty(nameof(targetObject.Tag59s)), new ParentPropertySetter())
          {
            IsEnumerable = true,
            InnerType = typeof(TestMany),
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
      var uut = new CompositePropertySetter();
      var valueToSet = new byte[1].AsSpan();
      var mappingTag60 = new TagMapLeaf(typeof(TestMany).GetProperty("Tag60"), new MockPropertySetter())
      {
        Parents = new List<TagMapParent>
       {
          new TagMapParent(targetObject.GetType().GetProperty(nameof(targetObject.Tag59s)),new ParentPropertySetter())
          {
            IsEnumerable = true,
            InnerType = typeof(TestMany),
          }
        }
      };
      var mappingTag601 = new TagMapLeaf(typeof(TestMany).GetProperty("Tag601"), new MockPropertySetter())
      {
        Parents = new List<TagMapParent>
        {
          new TagMapParent(targetObject.GetType().GetProperty(nameof(targetObject.Tag59s)),new ParentPropertySetter())
          {
            IsEnumerable = true,
            InnerType = typeof(TestMany),
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
      var uut = new CompositePropertySetter();
      var valueToSet = new byte[1].AsSpan();
      var mappingDetails = new TagMapLeaf(typeof(TestMany).GetProperty("Tag60"), new MockPropertySetter())
      {
        Parents = new List<TagMapParent>
        {
          new TagMapParent(targetObject.GetType().GetProperty(nameof(targetObject.Tag59s)),new ParentPropertySetter())
          {
            IsEnumerable = true,
            InnerType = typeof(TestMany),
          }
        }
      };
      var messageContext = new FixMessageContext();
      uut.Set(valueToSet, mappingDetails, messageContext, targetObject);
      var result = uut.Set(valueToSet, mappingDetails, messageContext, targetObject);

      Assert.NotNull(targetObject.Tag59s.First());
      Assert.Equal(result, targetObject.Tag59s.Skip(1).First());
    }

    [Fact]
    public void GivenParentsAsString_Set_CreatesParentTypes()
    {
      var targetObject = new TestTypeParent();
      var uut = new CompositePropertySetter();
      var valueToSet = "test";
      var mappingDetails = new TagMapLeaf(MockPropertyInfo.Get, new MockPropertySetter())
      {
        Parents = new List<TagMapParent>
        {
          new TagMapParent(targetObject.GetType().GetProperty(nameof(targetObject.CustomType)), new ParentPropertySetter())
        }
      };
      var messageContext = new FixMessageContext();
      var result = uut.Set(valueToSet.AsSpan(), mappingDetails, messageContext, targetObject);

      Assert.NotNull(targetObject.CustomType);
      Assert.Equal(result, targetObject.CustomType);
    }


    [Fact]
    public void GivenIntegerAndMappingDetails_Set_SetsValueOnTargetObject()
    {
      var targetObject = new TestTypeParent();
      var uut = new CompositePropertySetter();
      var valueToSet = Encoding.ASCII.GetBytes("12357").AsSpan();
      var mappingDetails = new TagMapLeaf(targetObject.GetType().GetProperty(nameof(targetObject.Tag62)), new IntegerSetter());
      var messageContext = new FixMessageContext();
      uut.Set(valueToSet, mappingDetails, messageContext, targetObject);

      Assert.Equal(12357, targetObject.Tag62);
    }

    [Fact]
    public void GivenNullable_Set_SetsValueOnTargetObject()
    {
      var targetObject = new TestTypeParent();
      var uut = new CompositePropertySetter();
      var valueToSet = Encoding.ASCII.GetBytes("12357").AsSpan();
      var mappingDetails = new TagMapLeaf(targetObject.GetType().GetProperty(nameof(targetObject.Tag63)), new IntegerSetter());
      var messageContext = new FixMessageContext();
      uut.Set(valueToSet, mappingDetails, messageContext, targetObject);

      Assert.Equal(12357, targetObject.Tag63);
    }

    [Fact]
    public void GivenEnumerableValue_Set_SetsValueOnTargetObjectsProperIndex()
    {
      var targetObject = new TestTypeParent() { Tag57s = new string[3] };
      var uut = new CompositePropertySetter();
      var valueToSet = Encoding.ASCII.GetBytes("12357").AsSpan();
      var mappingDetails = new TagMapLeaf(targetObject.GetType().GetProperty(nameof(targetObject.Tag57s)), new StringSetter())
      {
        IsEnumerable = true,
        RepeatingTagNumber = 56,
        InnerType = typeof(string),
      };
      var messageContext = new FixMessageContext();
      uut.Set(valueToSet, mappingDetails, messageContext, targetObject);

      Assert.Equal("12357", targetObject.Tag57s.First());
    }

    [Fact]
    public void GivenEnumerableAtSecond_Set_SetsValueOnTargetObjectsProperIndex()
    {
      var targetObject = new TestTypeParent() { Tag57s = new string[3] };
      var uut = new CompositePropertySetter();
      var valueToSet = Encoding.ASCII.GetBytes("12357").AsSpan();
      var mappingDetails = new TagMapLeaf(targetObject.GetType().GetProperty(nameof(targetObject.Tag57s)), new StringSetter())
      {
        IsEnumerable = true,
        RepeatingTagNumber = 56,
        InnerType = typeof(string),
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
      var uut = new CompositePropertySetter();
      var integerToSet = Encoding.ASCII.GetBytes("12357").AsSpan();
      var stringToSet = Encoding.ASCII.GetBytes("message").AsSpan();
      var doubleToSet = Encoding.ASCII.GetBytes("123.456").AsSpan();
      var mappingDetailsInt = new TagMapLeaf(targetObject.GetType().GetProperty(nameof(targetObject.Tag101)), new IntegerSetter());
      var mappingDetailsString = new TagMapLeaf(targetObject.GetType().GetProperty(nameof(targetObject.Tag100)), new StringSetter());
      var mappingDetailsDouble = new TagMapLeaf(targetObject.GetType().GetProperty(nameof(targetObject.Tag102)), new DoubleSetter());
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
      var uut = new CompositePropertySetter();
      var valueToSet = Encoding.ASCII.GetBytes("message").AsSpan();
      var mappingDetails = new TagMapLeaf(targetObject.GetType().GetProperty(nameof(targetObject.Tag104)), new StringSetter())
      {
        IsEnumerable = true,
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
      var uut = new CompositePropertySetter();
      var valueToSet = "12357".AsSpan();
      var mappingDetails = new TagMapLeaf(targetObject.GetType().GetProperty(nameof(targetObject.Tag62)), new IntegerSetter());
      var messageContext = new FixMessageContext();
      uut.Set(valueToSet, mappingDetails, messageContext, targetObject);

      Assert.Equal(12357, targetObject.Tag62);
    }

    [Fact]
    public void GivenNullableIntegerAsString_Set_SetsValueOnTargetObject()
    {
      var targetObject = new TestTypeParent();
      var uut = new CompositePropertySetter();
      ReadOnlySpan<char> valueToSet = "12357".AsSpan();
      var mappingDetails = new TagMapLeaf(targetObject.GetType().GetProperty(nameof(targetObject.Tag63)), new IntegerSetter());
      var messageContext = new FixMessageContext();
      uut.Set(valueToSet, mappingDetails, messageContext, targetObject);

      Assert.Equal(12357, targetObject.Tag63);
    }

    [Fact]
    public void GivenEnumerableAsString_Set_SetsValueOnTargetObjectsProperIndex()
    {
      var targetObject = new TestTypeParent() { Tag57s = new string[3] };
      var uut = new CompositePropertySetter();
      var firstValueToSet = "12357";
      var secondValueToSet = "12358";
      var mappingDetails = new TagMapLeaf(targetObject.GetType().GetProperty(nameof(targetObject.Tag57s)), new StringSetter())
      {
        IsEnumerable = true,
        RepeatingTagNumber = 56,
        InnerType = typeof(string),
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
      var uut = new CompositePropertySetter();
      var integerToSet = "12357".AsSpan();
      var stringToSet = "message".AsSpan();
      var doubleToSet = "123.456".AsSpan();
      var mappingDetailsInt = new TagMapLeaf(targetObject.GetType().GetProperty(nameof(targetObject.Tag101)), new IntegerSetter());
      var mappingDetailsString = new TagMapLeaf(targetObject.GetType().GetProperty(nameof(targetObject.Tag100)), new StringSetter());
      var mappingDetailsDouble = new TagMapLeaf(targetObject.GetType().GetProperty(nameof(targetObject.Tag102)), new DoubleSetter());
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
      var uut = new CompositePropertySetter();
      var valueToSet = "message";
      var mappingDetails = new TagMapLeaf(targetObject.GetType().GetProperty(nameof(targetObject.Tag104)), new StringSetter())
      {
        IsEnumerable = true,
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
      var uut = new CompositePropertySetter();
      var valueToSet = Encoding.ASCII.GetBytes("Y").AsSpan();
      var mappingDetails = new TagMapLeaf(targetObject.GetType().GetProperty(nameof(targetObject.Tag68)), new BooleanSetter());
      var messageContext = new FixMessageContext();
      uut.Set(valueToSet, mappingDetails, messageContext, targetObject);
      Assert.True(targetObject.Tag68);
    }

    [Fact]
    public void GivenNullableBool_Set_SetsValueOnTargetObject()
    {
      var targetObject = new TestTypeParent();
      var uut = new CompositePropertySetter();
      var valueToSet = Encoding.ASCII.GetBytes("Y").AsSpan();
      var mappingDetails = new TagMapLeaf(targetObject.GetType().GetProperty(nameof(targetObject.Tag69)), new NullableBooleanSetter());
      var messageContext = new FixMessageContext();
      uut.Set(valueToSet, mappingDetails, messageContext, targetObject);
      Assert.True(targetObject.Tag69);
      Assert.False(targetObject.Tag68);
    }

    [Fact]
    public void GivenBoolAsString_Set_SetsValueOnTargetObject()
    {
      var targetObject = new TestTypeParent();
      var uut = new CompositePropertySetter();
      var valueToSet = "Y";
      var mappingDetails = new TagMapLeaf(targetObject.GetType().GetProperty(nameof(targetObject.Tag68)), new BooleanSetter());
      var messageContext = new FixMessageContext();
      uut.Set(valueToSet.AsSpan(), mappingDetails, messageContext, targetObject);
      Assert.True(targetObject.Tag68);
    }

    [Fact]
    public void GivenNullableBoolAsString_Set_SetsValueOnTargetObject()
    {
      var targetObject = new TestTypeParent();
      var uut = new CompositePropertySetter();
      var valueToSet = "Y";
      var mappingDetails = new TagMapLeaf(targetObject.GetType().GetProperty(nameof(targetObject.Tag69)), new NullableBooleanSetter());
      var messageContext = new FixMessageContext();
      uut.Set(valueToSet.AsSpan(), mappingDetails, messageContext, targetObject);
      Assert.True(targetObject.Tag69);
      Assert.False(targetObject.Tag68);
    }

    [Fact]
    public void GivenDateTimeOffsetAsString_Set_SetsValueOnTargetObject()
    {
      var targetObject = new TestTypeParent();
      var uut = new CompositePropertySetter();
      var valueToSet = "20181219-18:14:23";
      var mappingDetails = new TagMapLeaf(targetObject.GetType().GetProperty(nameof(targetObject.Tag70)), new DateTimeOffsetSetter());
      var messageContext = new FixMessageContext();
      uut.Set(valueToSet.AsSpan(), mappingDetails, messageContext, targetObject);
      Assert.Equal(new DateTimeOffset(2018, 12, 19, 18, 14, 23, 0, TimeSpan.Zero), targetObject.Tag70);
    }

    [Fact]
    public void GivenNullableDateTimeOffsetAsString_Set_SetsValueOnTargetObject()
    {
      var targetObject = new TestTypeParent();
      var uut = new CompositePropertySetter();
      var valueToSet = "20181219-18:14:23";
      var mappingDetails = new TagMapLeaf(targetObject.GetType().GetProperty(nameof(targetObject.Tag71)), new NullableDateTimeOffsetSetter());
      var messageContext = new FixMessageContext();
      uut.Set(valueToSet.AsSpan(), mappingDetails, messageContext, targetObject);
      Assert.Equal(new DateTimeOffset(2018, 12, 19, 18, 14, 23, 0, TimeSpan.Zero), targetObject.Tag71.Value);
    }

    private class MockPropertyInfo
    {
      //This could be any propertyInfo.
      public static PropertyInfo Get => typeof(MockPropertyInfo).GetProperty("Get");
    }

    private class MockPropertySetter : BaseSetter, ITypedPropertySetter, IParentSetter
    {
      public bool IsVerified { get; private set; }
      public new object Set(ReadOnlySpan<byte> value, TagMapLeaf mappingDetails, FixMessageContext fixMessageContext, object targetObject)
      {
        IsVerified = true;
        return targetObject;
      }

      public new TTarget SetTarget<TTarget>(ReadOnlySpan<byte> value, TagMapLeaf mappingDetails, FixMessageContext fixMessageContext, ref TTarget targetObject)
      {
        IsVerified = true;
        return targetObject;
      }

      public override object Set(ReadOnlySpan<char> value, TagMapLeaf mappingDetails, FixMessageContext fixMessageContext, object targetObject)
      {
        IsVerified = true;
        return targetObject;
      }

      public override TTarget SetTarget<TTarget>(ReadOnlySpan<char> value, TagMapLeaf mappingDetails, FixMessageContext fixMessageContext, ref TTarget targetObject)
      {
        IsVerified = true;
        return targetObject;
      }

      public object Set(TagMapLeaf leaf, TagMapParent parent, FixMessageContext fixMessageContext, object targetObject)
      {
        IsVerified = true;
        return targetObject;
      }
    }

  }
}
