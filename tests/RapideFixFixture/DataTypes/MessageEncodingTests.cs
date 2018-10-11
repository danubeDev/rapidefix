using RapideFix.DataTypes;
using RapideFix.Extensions;
using System;
using System.Text;
using Xunit;

namespace RapideFixFixture.DataTypes
{
  public class MessageEncodingTests
  {
    [Fact]
    public void GivenEncoding_GetEncoder_NotNull()
    {
      //In .net core, we need to register CodePagesEncodingProvider and related NuGet package.
      Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
      Assert.NotNull(MessageEncoding.EUC.GetEncoder());
      Assert.NotNull(MessageEncoding.JIS.GetEncoder());
      Assert.NotNull(MessageEncoding.UTF8.GetEncoder());
    }

    [Fact]
    public void GivenEncoding_ToString_NotNull()
    {
      Assert.NotNull(MessageEncoding.EUC.ToString());
      Assert.NotNull(MessageEncoding.JIS.ToString());
      Assert.NotNull(MessageEncoding.UTF8.ToString());
    }

    [Fact]
    public void GivenInvalidBytes_Parse_ThrowsNotSupportedException()
    {
      Assert.Throws<NotSupportedException>(() => MessageEncoding.Parse((new byte[3]).AsSpan()));
    }

    [Fact]
    public void GivenInvalidBytes_TryParse_ReturnsFalse()
    {
      Assert.False(MessageEncoding.TryParse((new byte[3]).AsSpan(), out var dummy));
    }

    [Fact]
    public void GivenBytes_Parse_ReturnsEncoding()
    {
      Assert.NotNull(MessageEncoding.Parse("EUC-JP".ToByteValueAndSOH()));
      Assert.NotNull(MessageEncoding.Parse("ISO-2022-JP".ToByteValueAndSOH()));
      Assert.NotNull(MessageEncoding.Parse("UTF-8".ToByteValueAndSOH()));
    }

    [Fact]
    public void GivenBytes_TryParse_ReturnsTrue()
    {
      Assert.True(MessageEncoding.TryParse("EUC-JP".ToByteValueAndSOH(), out var dummyEUC));
      Assert.True(MessageEncoding.TryParse("ISO-2022-JP".ToByteValueAndSOH(), out var dummyJIS));
      Assert.True(MessageEncoding.TryParse("UTF-8".ToByteValueAndSOH(), out var dummyUTF8));
    }
  }
}
