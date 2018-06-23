using RapideFix.DataTypes;
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
    public void GivenEncoding_ToSting_NotNull()
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
    public void GivenBytes_TryParse_ReturnsFalse()
    {
      Assert.False(MessageEncoding.TryParse((new byte[3]).AsSpan(), out var dummy));
    }

    [Fact]
    public void GivenBytes_Parse_ReturnsEncoding()
    {
      Assert.NotNull(MessageEncoding.Parse(MessageEncoding.EUC.FixEncodedValue()));
      Assert.NotNull(MessageEncoding.Parse(MessageEncoding.JIS.FixEncodedValue()));
      Assert.NotNull(MessageEncoding.Parse(MessageEncoding.UTF8.FixEncodedValue()));
    }

    [Fact]
    public void GivenBytes_TryParse_ReturnsTrue()
    {
      Assert.True(MessageEncoding.TryParse(MessageEncoding.EUC.FixEncodedValue(), out var dummyEUC));
      Assert.True(MessageEncoding.TryParse(MessageEncoding.JIS.FixEncodedValue(), out var dummyJIS));
      Assert.True(MessageEncoding.TryParse(MessageEncoding.UTF8.FixEncodedValue(), out var dummyUTF8));
    }
  }
}
