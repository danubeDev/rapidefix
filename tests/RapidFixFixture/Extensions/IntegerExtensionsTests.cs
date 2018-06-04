using RapideFix;
using RapideFix.Extensions;
using System;
using System.Text;
using Xunit;

namespace RapideFixFixture.Extensions
{
  public class IntegerExtensionsTests
  {
    [Theory]
    [InlineData(9)]
    [InlineData(23)]
    public void GivenInteger_ToSOHAndKnownTag_ConvertsValue(int tag)
    {
      byte[] result = tag.ToSOHAndKnownTag();
      Assert.Equal(Constants.SOHByte, result[0]);
      var resultString = Encoding.ASCII.GetString(result.AsSpan(1));
      Assert.Equal($"{tag}=", resultString);
    }

    [Theory]
    [InlineData(8)]
    [InlineData(231)]
    public void GivenInteger_ToKnownTag_ConvertsValue(int tag)
    {
      byte[] result = tag.ToKnownTag();
      var resultString = Encoding.ASCII.GetString(result);
      Assert.Equal($"{tag}=", resultString);
    }
  }
}
