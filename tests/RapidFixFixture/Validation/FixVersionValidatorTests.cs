using RapideFix;
using RapideFix.Factories;
using RapideFix.Validation;
using System;
using Xunit;

namespace RapideFixFixture.Validation
{
  public class FixVersionValidatorTests
  {
    [Theory]
    [MemberData(nameof(SampleFixMessagesSource.GetSampleFixBodies), MemberType = typeof(SampleFixMessagesSource))]
    public void GivenValidFixVersion_Validate_ReturnsTrue(string input)
    {
      byte[] message = new TestFixMessageBuilder(input).Build();
      var uut = new FixVersionValidator();
      var msgContext = new MessageContextFactory().Create(message);
      var result = uut.IsValid(message.AsSpan(), msgContext);
      Assert.True(result);
    }

    [Fact]
    public void GivenInvalidVersion_Validate_ReturnsFalse()
    {
      var message = new TestFixMessageBuilder(TestFixMessageBuilder.DefaultBody).AddBeginString("8=123|").Build();
      var uut = new FixVersionValidator();
      var msgContext = new MessageContextFactory().Create(message);
      var result = uut.IsValid(message.AsSpan(), msgContext);
      Assert.False(result);
    }

    [Fact]
    public void GivenNoBeginString_Validate_ReturnsFalse()
    {
      var message = new TestFixMessageBuilder(TestFixMessageBuilder.DefaultBody).AddBeginString(string.Empty).Build();
      var uut = new FixVersionValidator();
      var msgContext = new MessageContextFactory().Create(message);
      var result = uut.IsValid(message.AsSpan(), msgContext);
      Assert.False(result);
    }

    [Fact]
    public void GivenFix5WithNoTargetCompId_Validate_ReturnsFalse()
    {
      var message = new TestFixMessageBuilder("35=A|49=SERVER|34=177|52=20090107-18:15:16|98=0|108=30|").AddBeginString(SupportedFixVersion.Fix50).Build();
      var uut = new FixVersionValidator();
      var msgContext = new MessageContextFactory().Create(message);
      var result = uut.IsValid(message.AsSpan(), msgContext);
      Assert.False(result);
    }

    [Fact]
    public void GivenFix5WithNoSenderCompId_Validate_ReturnsFalse()
    {
      var message = new TestFixMessageBuilder("35=A|56=CLIENT|34=177|52=20090107-18:15:16|98=0|108=30|").AddBeginString(SupportedFixVersion.Fix50).Build();
      var uut = new FixVersionValidator();
      var msgContext = new MessageContextFactory().Create(message);
      var result = uut.IsValid(message.AsSpan(), msgContext);
      Assert.False(result);
    }

  }
}
