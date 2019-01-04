using System;
using System.Collections.Generic;
using Moq;
using RapideFix.Business;
using RapideFix.Business.Data;
using RapideFix.Business.PropertySetters;
using RapideFix.DataTypes;
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
      uut.Set(testValue, new TagMapLeaf() { Parents = new List<TagMapNode>() { new TagMapNode() { ParentSetter = mockParentSetter } }, Setter = mockSimpleTypeSetter }, new FixMessageContext(), new object());

      Assert.True(mockParentSetter.IsVerified);
      Assert.True(mockSimpleTypeSetter.IsVerified);
    }

    [Fact]
    public void GivenSimpleValue_Set_CallsSimpleSetter()
    {
      var testValue = new byte[0];
      var mockSimpleTypeSetter = new MockPropertySetter();

      var uut = new CompositePropertySetter();
      uut.Set(testValue, new TagMapLeaf() { Setter = mockSimpleTypeSetter }, new FixMessageContext(), new object());

      Assert.True(mockSimpleTypeSetter.IsVerified);
    }

    [Fact]
    public void GivenRepeatingGroup_Set_CallsRepeatingSetter()
    {
      var testValue = new byte[0];
      var mockSetter = new MockPropertySetter();

      var uut = new CompositePropertySetter();
      uut.Set(testValue, TagMapLeaf.CreateRepeatingTag<TagMapLeaf>(null, null, mockSetter), new FixMessageContext(), new object());

      Assert.True(mockSetter.IsVerified);
    }

    [Fact]
    public void GivenTypeConvertedSetter_Set_CallsConvertingSetter()
    {
      var testValue = new byte[0];
      var mockSetter = new MockPropertySetter();

      var uut = new CompositePropertySetter();
      uut.Set(testValue, new TagMapLeaf() { TypeConverterName = "name", Setter = mockSetter }, new FixMessageContext(), new object());

      Assert.True(mockSetter.IsVerified);
    }

    [Fact]
    public void GivenTypedSimpleValue_SetTarget_CallsTypedSetter()
    {
      var testValue = new byte[0];
      var mockSimpleTypeSetter = new MockPropertySetter();

      var uut = new CompositePropertySetter();
      var targetObject = new object();
      uut.SetTarget(testValue, new TagMapLeaf() { Setter = mockSimpleTypeSetter }, new FixMessageContext(), ref targetObject);

      Assert.True(mockSimpleTypeSetter.IsVerified);
    }

    [Fact]
    public void GivenParentAsString_Set_CallsParentsSetter()
    {
      var testValue = "test".AsSpan();
      var mockParentSetter = new MockPropertySetter();
      var mockSimpleTypeSetter = new MockPropertySetter();

      var uut = new CompositePropertySetter();
      uut.Set(testValue, new TagMapLeaf() { Parents = new List<TagMapNode>() { new TagMapNode() { ParentSetter = mockParentSetter } }, Setter = mockSimpleTypeSetter }, new FixMessageContext(), new object());

      Assert.True(mockParentSetter.IsVerified);
      Assert.True(mockSimpleTypeSetter.IsVerified);
    }

    [Fact]
    public void GivenSimpleValueAsString_Set_CallsSimpleSetter()
    {
      var testValue = "test".AsSpan();
      var mockSimpleTypeSetter = new MockPropertySetter();

      var uut = new CompositePropertySetter();
      uut.Set(testValue, new TagMapLeaf() { Setter = mockSimpleTypeSetter }, new FixMessageContext(), new object());

      Assert.True(mockSimpleTypeSetter.IsVerified);
    }

    [Fact]
    public void GivenRepeatingGroupAsString_Set_CallsRepeatingSetter()
    {
      var testValue = "test".AsSpan();
      var mockSetter = new MockPropertySetter();

      var uut = new CompositePropertySetter();
      uut.Set(testValue, TagMapLeaf.CreateRepeatingTag<TagMapLeaf>(null, null, mockSetter), new FixMessageContext(), new object());

      Assert.True(mockSetter.IsVerified);
    }

    [Fact]
    public void GivenTypeConvertedSetterAsString_Set_CallsConvertingSetter()
    {
      var testValue = "test".AsSpan();
      var mockSetter = new MockPropertySetter();

      var uut = new CompositePropertySetter();
      uut.Set(testValue, new TagMapLeaf() { TypeConverterName = "name", Setter = mockSetter }, new FixMessageContext(), new object());

      Assert.True(mockSetter.IsVerified);
    }

    [Fact]
    public void GivenTypedSimpleValueAsString_SetTarget_CallsTypedSetter()
    {
      var testValue = "test".AsSpan();
      var mockSimpleTypeSetter = new MockPropertySetter();

      var uut = new CompositePropertySetter();
      var targetObject = new object();
      uut.SetTarget(testValue, new TagMapLeaf() { Setter = mockSimpleTypeSetter }, new FixMessageContext(), ref targetObject);

      Assert.True(mockSimpleTypeSetter.IsVerified);
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

      public object Set(TagMapLeaf leaf, TagMapNode parent, FixMessageContext fixMessageContext, object targetObject)
      {
        IsVerified = true;
        return targetObject;
      }
    }

  }
}
