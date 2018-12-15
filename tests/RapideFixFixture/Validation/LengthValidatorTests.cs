using System;
using System.Diagnostics;
using RapideFix;
using RapideFix.DataTypes;
using RapideFix.Extensions;
using RapideFix.Validation;
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
    [MemberData(nameof(SampleFixMessagesSource.GetSampleFixBodies), MemberType = typeof(SampleFixMessagesSource))]
    public void GivenCorrectLength_Validate_ReturnsTrue(string input)
    {
      byte[] message = new TestFixMessageBuilder(input).Build();
      var uut = new LengthValidator(IntegerToFixConverter.Instance);
      var msgContext = new FixMessageContext().Setup(message);
      var result = uut.IsValid(message.AsSpan(), msgContext);
      Assert.True(result);
    }

    [Fact]
    public void GivenCorrectLength_Validate_Performance()
    {
      byte[] message = TestFixMessageBuilder.CreateDefaultMessage();

      var uut = new LengthValidator(IntegerToFixConverter.Instance);
      var msgContext = new FixMessageContext().Setup(message);
      var result = uut.IsValid(message.AsSpan(), msgContext);
      Stopwatch sw = new Stopwatch();
      sw.Start();
      for(int i = 0; i < 100; i++)
      {
        result = uut.IsValid(message.AsSpan(), msgContext);
      }
      sw.Stop();

      _output.WriteLine($"==================={sw.ElapsedMilliseconds}======================");
    }

    [Fact]
    public void GivenIncorrectLength_Validate_ReturnsFalse()
    {
      var message = new TestFixMessageBuilder(TestFixMessageBuilder.DefaultBody).AddLength("023").Build();
      var uut = new LengthValidator(IntegerToFixConverter.Instance);
      var msgContext = new FixMessageContext().Setup(message);
      var result = uut.IsValid(message.AsSpan(), msgContext);
      Assert.False(result);
    }

    [Fact]
    public void GivenInvalidLength_Validate_ReturnsFalse()
    {
      var message = new TestFixMessageBuilder(TestFixMessageBuilder.DefaultBody).AddLength("").Build();
      var uut = new LengthValidator(IntegerToFixConverter.Instance);
      var msgContext = new FixMessageContext().Setup(message);
      var result = uut.IsValid(message.AsSpan(), msgContext);
      Assert.False(result);
    }

    [Fact]
    public void GivenNoLength_Validate_ReturnsFalse()
    {
      var message = new TestFixMessageBuilder(TestFixMessageBuilder.DefaultBody).AddLength("3").Build();
      var uut = new LengthValidator(IntegerToFixConverter.Instance);
      var msgContext = new FixMessageContext().Setup(message);
      var result = uut.IsValid(message.AsSpan(), msgContext);
      Assert.False(result);
    }

  }
}
