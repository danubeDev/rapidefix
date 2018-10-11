using System;
using Moq;
using RapideFix.Business;
using RapideFix.Business.Data;
using RapideFix.Parsers;
using RapideFix.Validation;
using RapideFixFixture.Mocks;
using RapideFixFixture.TestTypes;
using Xunit;

namespace RapideFixFixture.Parsers
{
  public class TypedMessageParserTests
  {
    private const string SampleBody = "35=A|100=Mesage|101=11|102=123.456|";

    [Fact]
    public void GivenNulls_Construct_ThrowsArgumentNullException()
    {
      Assert.Throws<ArgumentNullException>(() => new TypedMessageParser<TestTypeStruct>(null, Mock.Of<ITypedPropertySetter>(), Mock.Of<IValidator>(), Mock.Of<MessageParserOptions>()));
      Assert.Throws<ArgumentNullException>(() => new TypedMessageParser<TestTypeStruct>(Mock.Of<ITagToPropertyMapper>(), null, Mock.Of<IValidator>(), Mock.Of<MessageParserOptions>()));
      Assert.Throws<ArgumentNullException>(() => new TypedMessageParser<TestTypeStruct>(Mock.Of<ITagToPropertyMapper>(), Mock.Of<ITypedPropertySetter>(), null, Mock.Of<MessageParserOptions>()));
      Assert.Throws<ArgumentNullException>(() => new TypedMessageParser<TestTypeStruct>(Mock.Of<ITagToPropertyMapper>(), Mock.Of<ITypedPropertySetter>(), Mock.Of<IValidator>(), null));
    }

    [Fact]
    public void GivenDependencies_Construct_DoesNotThrow()
    {
      var ex = Record.Exception(() => new TypedMessageParser<TestTypeStruct>(Mock.Of<ITagToPropertyMapper>(), Mock.Of<ITypedPropertySetter>(), Mock.Of<IValidator>(), Mock.Of<MessageParserOptions>()));
      Assert.Null(ex);
    }

    [Fact]
    public void GivenMessage_Parse_ParsesStruct()
    {
      var propertyMapper = new TagToPropertyMapper();
      propertyMapper.Map<TestTypeStruct>();
      var compositeSetter = new CompositePropertySetter(new SubPropertySetterFactory());

      var uut = new TypedMessageParser<TestTypeStruct>(propertyMapper, compositeSetter, new MockValidator(), Mock.Of<MessageParserOptions>());
      var result = uut.Parse(new TestFixMessageBuilder(SampleBody).Build());

      Assert.NotNull(result.Tag100);
      Assert.NotEqual(0, result.Tag101);
      Assert.NotEqual(0, result.Tag101);
    }

    [Fact]
    public void GivenInValidWithNoThrow_Parse_DoesNotThrow()
    {
      var uut = new TypedMessageParser<TestTypeStruct>(Mock.Of<ITagToPropertyMapper>(), Mock.Of<ITypedPropertySetter>(), new MockValidator(false), Mock.Of<MessageParserOptions>());
      var ex = Record.Exception(() => uut.Parse(new TestFixMessageBuilder(SampleBody).Build()));
      Assert.Null(ex);
    }

    [Fact]
    public void GivenInValidWithThrow_Parse_ThrowsArgumentException()
    {
      var uut = new TypedMessageParser<TestTypeStruct>(Mock.Of<ITagToPropertyMapper>(), Mock.Of<ITypedPropertySetter>(), new MockValidator(false), new MessageParserOptions() { ThrowIfInvalidMessage = true });
      Assert.Throws<ArgumentException>(() => uut.Parse(new TestFixMessageBuilder(SampleBody).Build()));
    }

    [Theory]
    [MemberData(nameof(SampleFixMessagesSource.GetTestTypeParentMessageBodies), MemberType = typeof(SampleFixMessagesSource))]
    public void GivenMessage_Parse_ParsesReferenceType(string input)
    {
      byte[] message = new TestFixMessageBuilder(input).Build();
      var compositeSetter = new CompositePropertySetter(new SubPropertySetterFactory());

      var uut = new TypedMessageParser<TestTypeParent>(new TagToPropertyMapper(), compositeSetter, new MockValidator(), Mock.Of<MessageParserOptions>());
      var result = uut.Parse(message);
      Assert.NotNull(result);
      Assert.NotNull(result.Tag55);
    }

  }
}
