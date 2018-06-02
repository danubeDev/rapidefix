using RapideFix;
using RapideFix.Validation;
using System;
using Xunit;

namespace RapideFixFixture.Validation
{
  public class ChecksumValidatorTests
  {
    [Theory]
    [InlineData("")]
    [InlineData("35=8|49=PHLX|20=3|167=CS|54=1|38=15|58=PHLX EQUITY TESTING|59=0|47=C|32=0|31=0|151=15|14=0|6=0|")]
    [InlineData("35=8|49=PHLX|56=PERS|52=20071123-05:30:00.000|11=ATOMNOCCC9990900|20=3|150=E|39=E|55=MSFT|167=CS|54=1|38=15|40=2|44=15|58=PHLX EQUITY TESTING|59=0|47=C|32=0|31=0|151=15|14=0|6=0|")]
    public void GivenCorrectChecksum_Validate_ReturnsTrue(string input)
    {
      byte[] message;
      if(string.IsNullOrEmpty(input))
      {
        message = TestFixMessageBuilder.CreateDefaultMessage();
      }
      else
      {
        message = new TestFixMessageBuilder(input).Build();
      }

      var uut = new ChecksumValidator();
      var result = uut.IsValid(message.AsSpan());
      Assert.True(result);
    }

    [Fact]
    public void GivenIncorrectChecksum_Validate_ReturnsFalse()
    {
      var message = new TestFixMessageBuilder(TestFixMessageBuilder.DefaultBody).Build(checksum: "10=023|");
      var uut = new ChecksumValidator();
      var result = uut.IsValid(message.AsSpan());
      Assert.False(result);
    }

    [Fact]
    public void GivenInvalidChecksum_Validate_ReturnsFalse()
    {
      var message = new TestFixMessageBuilder(TestFixMessageBuilder.DefaultBody).Build(checksum: "10=3|");
      var uut = new ChecksumValidator();
      var result = uut.IsValid(message.AsSpan());
      Assert.False(result);
    }

    [Fact]
    public void GivenNoChecksum_Validate_ReturnsFalse()
    {
      var message = new TestFixMessageBuilder(TestFixMessageBuilder.DefaultBody).Build(checksum: "11=3|");
      var uut = new ChecksumValidator();
      var result = uut.IsValid(message.AsSpan());
      Assert.False(result);
    }

  }
}
