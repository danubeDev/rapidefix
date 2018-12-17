using System;
using System.Collections.Generic;
using Moq;
using RapideFix.Business;
using RapideFix.Business.Data;
using RapideFix.DataTypes;
using Xunit;

namespace RapideFixFixture.Business
{
  public class CompositePropertySetterTests
  {
    [Fact]
    public void GivenNullPropertySetterFactory_Construct_ThrowsArgumentNullException()
    {
      Assert.Throws<ArgumentNullException>(() => new CompositePropertySetter(null));
    }

    [Fact]
    public void GivenPropertySetterFactory_Construct_DoesNotThrow()
    {
      var ex = Record.Exception(() => new CompositePropertySetter(Mock.Of<ISubPropertySetterFactory>()));
      Assert.Null(ex);
    }

    [Fact]
    public void GivenParent_Set_CallsParentsSetter()
    {
      var testValue = new byte[0];
      var mockPropertyFactory = new Mock<ISubPropertySetterFactory>();
      var mockParentSetter = new MockPropertySetter();
      mockPropertyFactory.Setup(x => x.GetParentPropertySetter()).Returns(mockParentSetter);
      var mockSimpleTypeSetter = new MockPropertySetter();
      mockPropertyFactory.Setup(x => x.GetSimplePropertySetter()).Returns(mockSimpleTypeSetter);

      var uut = new CompositePropertySetter(mockPropertyFactory.Object);
      uut.Set(testValue, new TagMapLeaf() { Parents = new List<TagMapNode>() { new TagMapNode() } }, new FixMessageContext(), new object());

      Assert.True(mockParentSetter.IsVerified);
      Assert.True(mockSimpleTypeSetter.IsVerified);
    }

    [Fact]
    public void GivenSimpleValue_Set_CallsSimpleSetter()
    {
      var testValue = new byte[0];
      var mockPropertyFactory = new Mock<ISubPropertySetterFactory>();
      var mockSimpleTypeSetter = new MockPropertySetter();
      mockPropertyFactory.Setup(x => x.GetSimplePropertySetter()).Returns(mockSimpleTypeSetter);

      var uut = new CompositePropertySetter(mockPropertyFactory.Object);
      uut.Set(testValue, new TagMapLeaf(), new FixMessageContext(), new object());

      Assert.True(mockSimpleTypeSetter.IsVerified);
    }

    [Fact]
    public void GivenRepeatingGroup_Set_CallsRepeatingSetter()
    {
      var testValue = new byte[0];
      var mockPropertyFactory = new Mock<ISubPropertySetterFactory>();
      var mockSetter = new MockPropertySetter();
      mockPropertyFactory.Setup(x => x.GetRepeatingGroupTagPropertySetter()).Returns(mockSetter);

      var uut = new CompositePropertySetter(mockPropertyFactory.Object);
      uut.Set(testValue, TagMapLeaf.CreateRepeatingTag<TagMapLeaf>(null, null), new FixMessageContext(), new object());

      Assert.True(mockSetter.IsVerified);
    }

    [Fact]
    public void GivenTypeConvertedSetter_Set_CallsConvertingSetter()
    {
      var testValue = new byte[0];
      var mockPropertyFactory = new Mock<ISubPropertySetterFactory>();
      var mockSetter = new MockPropertySetter();
      mockPropertyFactory.Setup(x => x.GetTypeConvertedPropertySetter()).Returns(mockSetter);

      var uut = new CompositePropertySetter(mockPropertyFactory.Object);
      uut.Set(testValue, new TagMapLeaf() { TypeConverterName = "name" }, new FixMessageContext(), new object());

      Assert.True(mockSetter.IsVerified);
    }

    [Fact]
    public void GivenTypedSimpleValue_SetTarget_CallsTypedSetter()
    {
      var testValue = new byte[0];
      var mockPropertyFactory = new Mock<ISubPropertySetterFactory>();
      var mockSimpleTypeSetter = new MockPropertySetter();
      mockPropertyFactory.Setup(x => x.GetTypedPropertySetter()).Returns(mockSimpleTypeSetter);

      var uut = new CompositePropertySetter(mockPropertyFactory.Object);
      var targetObject = new object();
      uut.SetTarget(testValue, new TagMapLeaf(), new FixMessageContext(), ref targetObject);

      Assert.True(mockSimpleTypeSetter.IsVerified);
    }

    [Fact]
    public void GivenParentAsString_Set_CallsParentsSetter()
    {
      var testValue = "test".AsSpan();
      var mockPropertyFactory = new Mock<ISubPropertySetterFactory>();
      var mockParentSetter = new MockPropertySetter();
      mockPropertyFactory.Setup(x => x.GetParentPropertySetter()).Returns(mockParentSetter);
      var mockSimpleTypeSetter = new MockPropertySetter();
      mockPropertyFactory.Setup(x => x.GetSimplePropertySetter()).Returns(mockSimpleTypeSetter);

      var uut = new CompositePropertySetter(mockPropertyFactory.Object);
      uut.Set(testValue, new TagMapLeaf() { Parents = new List<TagMapNode>() { new TagMapNode() } }, new FixMessageContext(), new object());

      Assert.True(mockParentSetter.IsVerified);
      Assert.True(mockSimpleTypeSetter.IsVerified);
    }

    [Fact]
    public void GivenSimpleValueAsString_Set_CallsSimpleSetter()
    {
      var testValue = "test".AsSpan();
      var mockPropertyFactory = new Mock<ISubPropertySetterFactory>();
      var mockSimpleTypeSetter = new MockPropertySetter();
      mockPropertyFactory.Setup(x => x.GetSimplePropertySetter()).Returns(mockSimpleTypeSetter);

      var uut = new CompositePropertySetter(mockPropertyFactory.Object);
      uut.Set(testValue, new TagMapLeaf(), new FixMessageContext(), new object());

      Assert.True(mockSimpleTypeSetter.IsVerified);
    }

    [Fact]
    public void GivenRepeatingGroupAsString_Set_CallsRepeatingSetter()
    {
      var testValue = "test".AsSpan();
      var mockPropertyFactory = new Mock<ISubPropertySetterFactory>();
      var mockSetter = new MockPropertySetter();
      mockPropertyFactory.Setup(x => x.GetRepeatingGroupTagPropertySetter()).Returns(mockSetter);

      var uut = new CompositePropertySetter(mockPropertyFactory.Object);
      uut.Set(testValue, TagMapLeaf.CreateRepeatingTag<TagMapLeaf>(null, null), new FixMessageContext(), new object());

      Assert.True(mockSetter.IsVerified);
    }

    [Fact]
    public void GivenTypeConvertedSetterAsString_Set_CallsConvertingSetter()
    {
      var testValue = "test".AsSpan();
      var mockPropertyFactory = new Mock<ISubPropertySetterFactory>();
      var mockSetter = new MockPropertySetter();
      mockPropertyFactory.Setup(x => x.GetTypeConvertedPropertySetter()).Returns(mockSetter);

      var uut = new CompositePropertySetter(mockPropertyFactory.Object);
      uut.Set(testValue, new TagMapLeaf() { TypeConverterName = "name" }, new FixMessageContext(), new object());

      Assert.True(mockSetter.IsVerified);
    }

    [Fact]
    public void GivenTypedSimpleValueAsString_SetTarget_CallsTypedSetter()
    {
      var testValue = "test".AsSpan();
      var mockPropertyFactory = new Mock<ISubPropertySetterFactory>();
      var mockSimpleTypeSetter = new MockPropertySetter();
      mockPropertyFactory.Setup(x => x.GetTypedPropertySetter()).Returns(mockSimpleTypeSetter);

      var uut = new CompositePropertySetter(mockPropertyFactory.Object);
      var targetObject = new object();
      uut.SetTarget(testValue, new TagMapLeaf(), new FixMessageContext(), ref targetObject);

      Assert.True(mockSimpleTypeSetter.IsVerified);
    }

    private class MockPropertySetter : ITypedPropertySetter
    {
      public bool IsVerified { get; private set; }
      public object Set(ReadOnlySpan<byte> value, TagMapLeaf mappingDetails, FixMessageContext fixMessageContext, object targetObject)
      {
        IsVerified = true;
        return targetObject;
      }

      public TTarget SetTarget<TTarget>(ReadOnlySpan<byte> value, TagMapLeaf mappingDetails, FixMessageContext fixMessageContext, ref TTarget targetObject)
      {
        IsVerified = true;
        return targetObject;
      }

      public object Set(ReadOnlySpan<char> value, TagMapLeaf mappingDetails, FixMessageContext fixMessageContext, object targetObject)
      {
        IsVerified = true;
        return targetObject;
      }

      public TTarget SetTarget<TTarget>(ReadOnlySpan<char> value, TagMapLeaf mappingDetails, FixMessageContext fixMessageContext, ref TTarget targetObject)
      {
        IsVerified = true;
        return targetObject;
      }
    }

  }
}
