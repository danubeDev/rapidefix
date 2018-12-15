using System;
using System.Text;
using RapideFix.DataTypes;
using RapideFix.Extensions;
using Xunit;

namespace RapideFixFixture.Extensions
{
  public class StringExtensionsTests
  {
    [Theory]
    [InlineData("value")]
    [InlineData("otherValue")]
    public void GivenString_ToByteValueAndSOH_ReturnsBytesAndSOH(string value)
    {
      byte[] result = value.ToByteValueAndSOH();
      Assert.Equal(Constants.SOHByte, result[result.Length - 1]);
      var resultString = Encoding.ASCII.GetString(result.AsSpan(0, result.Length - 1));
      Assert.Equal(value, resultString);
    }

  }
}
