using RapideFix;
using RapideFix.Factories;
using RapideFix.Validation;
using System;
using Xunit;

namespace RapideFixFixture.Validation
{
  public class ChecksumValidatorTests
  {
    [Theory]
    [MemberData(nameof(SampleFixMessagesSource.GetSampleFixBodies), MemberType = typeof(SampleFixMessagesSource))]
    public void GivenCorrectChecksum_Validate_ReturnsTrue(string input)
    {
      byte[] message = new TestFixMessageBuilder(input).Build();
      var uut = new ChecksumValidator(new IntegerToFixConverter());
      var msgContext = new MessageContextFactory().Create(message);
      var result = uut.IsValid(message.AsSpan(), msgContext);
      Assert.True(result);
    }

    [Fact]
    public void GivenIncorrectChecksum_Validate_ReturnsFalse()
    {
      var message = new TestFixMessageBuilder(TestFixMessageBuilder.DefaultBody).AddChecksum("10=023|").Build();
      var uut = new ChecksumValidator(new IntegerToFixConverter());
      var msgContext = new MessageContextFactory().Create(message);
      var result = uut.IsValid(message.AsSpan(), msgContext);
      Assert.False(result);
    }

    [Fact]
    public void GivenInvalidChecksum_Validate_ReturnsFalse()
    {
      var message = new TestFixMessageBuilder(TestFixMessageBuilder.DefaultBody).AddChecksum("10=3|").Build();
      var uut = new ChecksumValidator(new IntegerToFixConverter());
      var msgContext = new MessageContextFactory().Create(message);
      var result = uut.IsValid(message.AsSpan(), msgContext);
      Assert.False(result);
    }

    [Fact]
    public void GivenNoChecksum_Validate_ReturnsFalse()
    {
      var message = new TestFixMessageBuilder(TestFixMessageBuilder.DefaultBody).AddChecksum("10=3|").Build();
      var uut = new ChecksumValidator(new IntegerToFixConverter());
      var msgContext = new MessageContextFactory().Create(message);
      var result = uut.IsValid(message.AsSpan(), msgContext);
      Assert.False(result);
    }

  }
}
