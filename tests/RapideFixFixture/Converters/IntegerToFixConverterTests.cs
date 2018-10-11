using System;
using System.Text;
using RapideFix;
using Xunit;

namespace RapideFixFixture.Converters
{
  public class IntegerToFixConverterTests
  {
    [Theory]
    [InlineData(58)]
    [InlineData(312)]
    public void GivenInteger_Convert_FillsByteArray(int testValue)
    {
      var uut = IntegerToFixConverter.Instance;
      byte[] expected = Encoding.ASCII.GetBytes(testValue.ToString());
      var result = new byte[expected.Length];
      uut.Convert(testValue, result, expected.Length);

      Assert.True(result.AsSpan().SequenceEqual(expected));
    }

    [Fact]
    public void GivenIntegerAndLargerSpan_Convert_FillsByteArray()
    {
      var uut = IntegerToFixConverter.Instance;
      byte[] expected = Encoding.ASCII.GetBytes("0023");
      var result = new byte[expected.Length];
      uut.Convert(23, result, expected.Length);

      Assert.True(result.AsSpan().SequenceEqual(expected));
    }

    [Theory]
    [InlineData("0023")]
    [InlineData("458")]
    public void GivenBytes_ConvertBack_ReturnsInteger(string testValue)
    {
      var uut = IntegerToFixConverter.Instance;
      byte[] testData = Encoding.ASCII.GetBytes(testValue);
      int result = uut.ConvertBack(testData);

      Assert.Equal(int.Parse(testValue), result);
    }
    
  }
}
