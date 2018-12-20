using System;
using Moq;
using RapideFix.Business;
using RapideFix.Business.Data;
using RapideFix.ParserBuilders;
using RapideFix.Parsers;
using RapideFix.Validation;
using RapideFixFixture.TestTypes;
using Xunit;

namespace RapideFixFixture.ParserBuilders
{
  public class ParserBuildersTests
  {
    [Fact]
    public void When_Construct_DoesNotThrow()
    {
      var ex = Record.Exception(() => new ParserBuilder<TestTypeParent>());
      Assert.Null(ex);
    }

    [Fact]
    public void NoSettings_Build_DoesNotThrow()
    {
      var ex = Record.Exception(() => new ParserBuilder<TestTypeParent>().Build<byte>());
      Assert.Null(ex);
    }

    [Fact]
    public void BuildParams_Build_ReturnsParser()
    {
      var parser = new ParserBuilder<TestTypeParent>()
        .Build<byte>(new MessageParserOptions() { ThrowIfInvalidMessage = true });
      Assert.NotNull(parser);
      Assert.IsType<TypedMessageParser<TestTypeParent>>(parser);
    }

    [Fact]
    public void BuildWithPropertyMapperParams_Build_ReturnsParser()
    {
      var parser = new ParserBuilder<TestTypeParent>()
        .Build<byte>(Mock.Of<ITagToPropertyMapper>());
      Assert.NotNull(parser);
      Assert.IsType<TypedMessageParser<TestTypeParent>>(parser);
    }

    [Fact]
    public void ObjectFromByte_Build_ReturnsParser()
    {
      var parser = new ParserBuilder<object>()
        .Build<byte>();
      Assert.NotNull(parser);
      Assert.IsType<MessageParser>(parser);
    }

    [Fact]
    public void ObjectFromInt_Build_ThrowsNotSupportedException()
    {
      Assert.Throws<NotSupportedException>(() => new ParserBuilder<object>().Build<int>());
    }

    [Fact]
    public void BuildWithAllParams_Build_ReturnsParser()
    {
      var parser = new ParserBuilder<TestTypeParent>()
        .Build<byte>(Mock.Of<ITagToPropertyMapper>(), Mock.Of<ITypedPropertySetter>(), Mock.Of<IValidator>(), new MessageParserOptions());
      Assert.NotNull(parser);
      Assert.IsType<TypedMessageParser<TestTypeParent>>(parser);
    }

    [Fact]
    public void BuildSetPropertyMapper_Build_ReturnsParser()
    {
      var mockPropertySetter = new Mock<ITagToPropertyMapper>();
      var parser = new ParserBuilder<TestTypeParent>().SetPropertyMapper(mockPropertySetter.Object).Build<byte>();
      Assert.NotNull(parser);
      Assert.IsType<TypedMessageParser<TestTypeParent>>(parser);
      mockPropertySetter.Verify(x => x.Map<TestTypeParent>());
    }

    [Fact]
    public void GivenNull_SetPropertyMapper_ThrowsArugmentNullException()
    {
      Assert.Throws<ArgumentNullException>(() => new ParserBuilder<TestTypeParent>().SetPropertyMapper(null));
    }

    [Fact]
    public void SetPropertyMapper_CalledTwice_ThrowsArgumentException()
    {
      Assert.Throws<ArgumentException>(() => new ParserBuilder<TestTypeParent>()
        .SetPropertyMapper(Mock.Of<ITagToPropertyMapper>())
        .SetPropertyMapper(Mock.Of<ITagToPropertyMapper>()));
    }

    [Fact]
    public void BuildSetPropertySetter_Build_ReturnsParser()
    {
      var parser = new ParserBuilder<TestTypeParent>().SetPropertySetter(Mock.Of<ITypedPropertySetter>()).Build<byte>();
      Assert.NotNull(parser);
      Assert.IsType<TypedMessageParser<TestTypeParent>>(parser);
    }

    [Fact]
    public void SetPropertySetter_CalledTwice_ThrowsArgumentException()
    {
      Assert.Throws<ArgumentException>(() => new ParserBuilder<TestTypeParent>()
        .SetPropertySetter(Mock.Of<ITypedPropertySetter>())
        .SetPropertySetter(Mock.Of<ITypedPropertySetter>()));
    }

    [Fact]
    public void GivenNull_SetPropertySetter_ThrowsArugmentNullException()
    {
      Assert.Throws<ArgumentNullException>(() => new ParserBuilder<TestTypeParent>().SetPropertySetter(null));
    }

    [Fact]
    public void BuildSetSubPropertySetter_Build_ReturnsParser()
    {
      var parser = new ParserBuilder<TestTypeParent>().SetSubPropertySetter(Mock.Of<ISubPropertySetterFactory>()).Build<byte>();
      Assert.NotNull(parser);
      Assert.IsType<TypedMessageParser<TestTypeParent>>(parser);
    }

    [Fact]
    public void SetSubPropertySetter_CalledTwice_ThrowsArgumentException()
    {
      Assert.Throws<ArgumentException>(() => new ParserBuilder<TestTypeParent>()
        .SetSubPropertySetter(Mock.Of<ISubPropertySetterFactory>())
        .SetSubPropertySetter(Mock.Of<ISubPropertySetterFactory>()));
    }

    [Fact]
    public void GivenNull_SetSubPropertySetter_ThrowsArugmentNullException()
    {
      Assert.Throws<ArgumentNullException>(() => new ParserBuilder<TestTypeParent>().SetSubPropertySetter(null));
    }

    [Fact]
    public void BuildSetValidators_Build_ReturnsParser()
    {
      var parser = new ParserBuilder<TestTypeParent>().SetValidators(Mock.Of<IValidator>()).Build<byte>();
      Assert.NotNull(parser);
      Assert.IsType<TypedMessageParser<TestTypeParent>>(parser);
    }

    [Fact]
    public void SetValidators_CalledTwice_ThrowsArgumentException()
    {
      Assert.Throws<ArgumentException>(() => new ParserBuilder<TestTypeParent>()
        .SetValidators(Mock.Of<IValidator>())
        .SetValidators(Mock.Of<IValidator>()));
    }

    [Fact]
    public void GivenNull_SetValidators_ThrowsArugmentNullException()
    {
      Assert.Throws<ArgumentNullException>(() => new ParserBuilder<TestTypeParent>().SetValidators(null));
    }

    [Fact]
    public void BuildSetOptions_Build_ReturnsParser()
    {
      var parser = new ParserBuilder<TestTypeParent>().SetOptions(new MessageParserOptions()).Build<byte>();
      Assert.NotNull(parser);
      Assert.IsType<TypedMessageParser<TestTypeParent>>(parser);
    }

    [Fact]
    public void SetOptions_CalledTwice_ThrowsArgumentException()
    {
      Assert.Throws<ArgumentException>(() => new ParserBuilder<TestTypeParent>()
        .SetOptions(new MessageParserOptions())
        .SetOptions(new MessageParserOptions()));
    }

    [Fact]
    public void GivenNull_SetOptions_ThrowsArugmentNullException()
    {
      Assert.Throws<ArgumentNullException>(() => new ParserBuilder<TestTypeParent>().SetOptions(null));
    }

    [Fact]
    public void SetPropertyMapper_AddOutputType_CallsPropertyMapper()
    {
      var mockPropertySetter = new Mock<ITagToPropertyMapper>();
      var parser = new ParserBuilder<TestTypeParent>()
        .SetPropertyMapper(mockPropertySetter.Object)
        .AddOutputType<TestTypeParent>()
        .AddOutputType<TestChild>()
        .Build<byte>();
      mockPropertySetter.Verify(x => x.Map<TestTypeParent>(), Times.AtLeast(2));
      mockPropertySetter.Verify(x => x.Map<TestChild>(), Times.Exactly(1));
    }

    [Fact]
    public void AddOutputType_SetPropertyMapper_ThrowsArugmentxception()
    {
      Assert.Throws<ArgumentException>(
        () => new ParserBuilder<TestTypeParent>()
        .AddOutputType<TestTypeParent>()
        .SetPropertyMapper(null));
    }
  }
}
