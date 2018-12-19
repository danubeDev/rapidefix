using System;
using System.Linq;
using System.Text;
using RapideFix.DataTypes;
using RapideFix.MessageBuilders;
using Xunit;

namespace RapideFixFixture.MessageBuilders
{
  public class FixTagValueConverterTests
  {
    [Fact]
    public void GivenString_Get_EncodesByteArray()
    {
      var uut = new FixTagValueConverter();
      byte[] into = new byte[6];
      uut.Get(9, "123", into);
      Assert.True(ConverterTestHelper.GetEncodedMessage("9=123|").SequenceEqual(into));
    }

    [Fact]
    public void GivenEncodingString_Get_EncodesByteArray()
    {
      var uut = new FixTagValueConverter();
      byte[] into = new byte[8];
      uut.Get(9, "tést", MessageEncoding.UTF8, into);
      Assert.True(ConverterTestHelper.GetEncodedMessage("9=t", Encoding.UTF8.GetBytes("é"), "st|").SequenceEqual(into));
    }

    [Fact]
    public void GivenInt_Get_EncodesByteArray()
    {
      var uut = new FixTagValueConverter();
      byte[] into = new byte[6];
      uut.Get(9, 123, into);
      Assert.True(ConverterTestHelper.GetEncodedMessage("9=123|").SequenceEqual(into));
    }

    [Fact]
    public void GivenIntFixedLength_Get_EncodesByteArray()
    {
      var uut = new FixTagValueConverter();
      byte[] into = new byte[6];
      uut.Get(9, 1, into, 3);
      Assert.True(ConverterTestHelper.GetEncodedMessage("9=001|").SequenceEqual(into));
    }

    [Fact]
    public void GivenDouble_Get_EncodesByteArray()
    {
      var uut = new FixTagValueConverter();
      byte[] into = new byte[7];
      uut.Get(9, 67.8, into);
      Assert.True(ConverterTestHelper.GetEncodedMessage("9=67.8|").SequenceEqual(into));
    }

    [Fact]
    public void GivenRawString_Get_EncodesByteArray()
    {
      var uut = new FixTagValueConverter();
      byte[] into = new byte[6];
      uut.Get("9=123|", into);
      Assert.True(ConverterTestHelper.GetEncodedMessage("9=123|").SequenceEqual(into));
    }

  }
}
