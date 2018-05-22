using RapideFix;
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
      var result = uut.IsValid(message.AsSpan());
      Assert.True(result);
    }

    [Fact]
    public void GivenIncorrectChecksum_Validate_ReturnsFalse()
    {
      var message = new TestFixMessageBuilder(TestFixMessageBuilder.DefaultBody).Build(checksum: "10=023|");
      var uut = new ChecksumValidator(new IntegerToFixConverter());
      var result = uut.IsValid(message.AsSpan());
      Assert.False(result);
    }

    [Fact]
    public void GivenInvalidChecksum_Validate_ReturnsFalse()
    {
      var message = new TestFixMessageBuilder(TestFixMessageBuilder.DefaultBody).Build(checksum: "10=3|");
      var uut = new ChecksumValidator(new IntegerToFixConverter());
      var result = uut.IsValid(message.AsSpan());
      Assert.False(result);
    }

    [Fact]
    public void GivenNoChecksum_Validate_ReturnsFalse()
    {
      var message = new TestFixMessageBuilder(TestFixMessageBuilder.DefaultBody).Build(checksum: "11=3|");
      var uut = new ChecksumValidator(new IntegerToFixConverter());
      var result = uut.IsValid(message.AsSpan());
      Assert.False(result);
    }

  }
}
