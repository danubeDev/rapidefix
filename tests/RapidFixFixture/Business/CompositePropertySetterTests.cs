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
      uut.Set(testValue, new TagMapLeaf() { Parents = new List<TagMapNode>() }, new FixMessageContext(), new object());

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
      uut.Set(testValue, new RepeatingGroupTagMapLeaf(), new FixMessageContext(), new object());

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

    private class MockPropertySetter : IPropertySetter
    {
      public bool IsVerified { get; private set; }
      public object Set(Span<byte> value, TagMapLeaf mappingDetails, FixMessageContext fixMessageContext, object targetObject)
      {
        IsVerified = true;
        return targetObject;
      }
    }
  }
}
