using System;
using Moq;
using RapideFix;
using RapideFix.Business;
using RapideFix.Business.Data;
using RapideFix.Parsers;
using RapideFix.Validation;
using RapideFixFixture.Mocks;
using RapideFixFixture.TestTypes;
using Xunit;

namespace RapideFixFixture.Parsers
{
  public class MessageParserTests
  {
    [Fact]
    public void GivenNulls_Construct_ThrowsArgumentNullException()
    {
      Assert.Throws<ArgumentNullException>(() => new MessageParser(null, Mock.Of<IPropertySetter>(), Mock.Of<IValidator>(), Mock.Of<MessageParserOptions>()));
      Assert.Throws<ArgumentNullException>(() => new MessageParser(Mock.Of<ITagToPropertyMapper>(), null, Mock.Of<IValidator>(), Mock.Of<MessageParserOptions>()));
      Assert.Throws<ArgumentNullException>(() => new MessageParser(Mock.Of<ITagToPropertyMapper>(), Mock.Of<IPropertySetter>(), null, Mock.Of<MessageParserOptions>()));
      Assert.Throws<ArgumentNullException>(() => new MessageParser(Mock.Of<ITagToPropertyMapper>(), Mock.Of<IPropertySetter>(), Mock.Of<IValidator>(), null));
    }

    [Fact]
    public void GivenDependencies_Construct_DoesNotThrow()
    {
      var ex = Record.Exception(() => new MessageParser(Mock.Of<ITagToPropertyMapper>(), Mock.Of<IPropertySetter>(), Mock.Of<IValidator>(), Mock.Of<MessageParserOptions>()));
      Assert.Null(ex);
    }

    [Fact]
    public void GivenMessage_Parse_ParsesObject()
    {
      var propertyMapper = new TagToPropertyMapper();
      propertyMapper.Map<TestTypeParent>();
      var compositeSetter = new CompositePropertySetter(new SubPropertySetterFactory());

      var uut = new MessageParser(propertyMapper, compositeSetter, new MockValidator(), Mock.Of<MessageParserOptions>());
      var result = uut.Parse<TestTypeParent>(TestFixMessageBuilder.CreateDefaultMessage());

      Assert.NotNull(result);
    }

    [Fact]
    public void GivenInValidWithNoThrow_Parse_DoesNotThrow()
    {
      var uut = new MessageParser(Mock.Of<ITagToPropertyMapper>(), Mock.Of<IPropertySetter>(), new MockValidator(false), Mock.Of<MessageParserOptions>());
      var result = uut.Parse<TestTypeParent>(TestFixMessageBuilder.CreateDefaultMessage());
      Assert.Null(result);
    }

    [Fact]
    public void GivenInValidWithThrow_Parse_ThrowsArgumentException()
    {
      var uut = new MessageParser(Mock.Of<ITagToPropertyMapper>(), Mock.Of<IPropertySetter>(), new MockValidator(false), new MessageParserOptions() { ThrowIfInvalidMessage = true });
      Assert.Throws<ArgumentException>(() => uut.Parse<TestTypeParent>(TestFixMessageBuilder.CreateDefaultMessage()));
    }

    [Theory]
    [MemberData(nameof(SampleFixMessagesSource.GetTestTypeParentMessageBodies), MemberType = typeof(SampleFixMessagesSource))]
    public void GivenSampleMessage_ParseT_ParsesObject(string input)
    {
      byte[] message = new TestFixMessageBuilder(input).Build();
      var propertyMapper = new TagToPropertyMapper();
      propertyMapper.Map<TestTypeParent>();
      var compositeSetter = new CompositePropertySetter(new SubPropertySetterFactory());

      var uut = new MessageParser(propertyMapper, compositeSetter, new MockValidator(), Mock.Of<MessageParserOptions>());
      var result = uut.Parse<TestTypeParent>(message);

      Assert.NotNull(result);
      Assert.NotNull(result.Tag55);
    }

    [Theory]
    [MemberData(nameof(SampleFixMessagesSource.GetTestTypeParentMessageBodies), MemberType = typeof(SampleFixMessagesSource))]
    public void GivenSampleMessage_Parse_ParsesObject(string input)
    {
      byte[] message = new TestFixMessageBuilder(input).Build();
      var propertyMapper = new TagToPropertyMapper();
      propertyMapper.Map<TestTypeParent>();
      var compositeSetter = new CompositePropertySetter(new SubPropertySetterFactory());

      var uut = new MessageParser(propertyMapper, compositeSetter, new MockValidator(), Mock.Of<MessageParserOptions>());
      var result = uut.Parse(message) as TestTypeParent;

      Assert.NotNull(result);
      Assert.NotNull(result.Tag55);
    }

    [Theory]
    [MemberData(nameof(SampleFixMessagesSource.GetTestTypeParentMessageBodies), MemberType = typeof(SampleFixMessagesSource))]
    public void GivenSampleMessageAndObject_Parse_ParsesObject(string input)
    {
      byte[] message = new TestFixMessageBuilder(input).Build();
      var propertyMapper = new TagToPropertyMapper();
      propertyMapper.Map<TestTypeParent>();
      var compositeSetter = new CompositePropertySetter(new SubPropertySetterFactory());

      var uut = new MessageParser(propertyMapper, compositeSetter, new MockValidator(), Mock.Of<MessageParserOptions>());
      TestTypeParent result = uut.Parse(message, new TestTypeParent());

      Assert.NotNull(result);
      Assert.NotNull(result.Tag55);
    }

  }
}
