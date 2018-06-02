using System;
using System.Diagnostics;
using Xunit;
using Xunit.Abstractions;

namespace RapideFixFixture.Validation
{
  public class LengthValidatorTests
  {
    private readonly ITestOutputHelper _output;
    public LengthValidatorTests(ITestOutputHelper output)
    {
      _output = output;
    }

    [Theory]
    [InlineData("")]
    [InlineData("35=8|49=PHLX|20=3|167=CS|54=1|38=15|58=PHLX EQUITY TESTING|59=0|47=C|32=0|31=0|151=15|14=0|6=0|")]
    [InlineData("35=8|49=PHLX|56=PERS|52=20071123-05:30:00.000|11=ATOMNOCCC9990900|20=3|150=E|39=E|55=MSFT|167=CS|54=1|38=15|40=2|44=15|58=PHLX EQUITY TESTING|59=0|47=C|32=0|31=0|151=15|14=0|6=0|")]
    public void GivenCorrectLength_Validate_ReturnsTrue(string input)
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

      var uut = new LengthValidator();
      var result = uut.IsValid(message.AsSpan());
      Assert.True(result);
    }

    [Fact]
    public void GivenCorrectLength_Validate_Performance()
    {
      byte[] message = TestFixMessageBuilder.CreateDefaultMessage();

      var uut = new LengthValidator();
      var result = uut.IsValid(message.AsSpan());
      Stopwatch sw = new Stopwatch();
      sw.Start();
      for(int i = 0; i < 100; i++)
      {
        result = uut.IsValid(message.AsSpan());
      }
      sw.Stop();

      _output.WriteLine($"==================={sw.ElapsedMilliseconds}======================");
    }

    [Fact]
    public void GivenIncorrectLength_Validate_ReturnsFalse()
    {
      var message = new TestFixMessageBuilder(TestFixMessageBuilder.DefaultBody).AddLength("9=023|").Build();
      var uut = new LengthValidator();
      var result = uut.IsValid(message.AsSpan());
      Assert.False(result);
    }

    [Fact]
    public void GivenInvalidLength_Validate_ReturnsFalse()
    {
      var message = new TestFixMessageBuilder(TestFixMessageBuilder.DefaultBody).AddLength("9=|").Build();
      var uut = new LengthValidator();
      var result = uut.IsValid(message.AsSpan());
      Assert.False(result);
    }

    [Fact]
    public void GivenNoLength_Validate_ReturnsFalse()
    {
      var message = new TestFixMessageBuilder(TestFixMessageBuilder.DefaultBody).AddLength("9=3|").Build();
      var uut = new LengthValidator();
      var result = uut.IsValid(message.AsSpan());
      Assert.False(result);
    }

  }
}
