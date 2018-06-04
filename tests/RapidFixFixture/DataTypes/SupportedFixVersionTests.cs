using RapideFix;
using RapideFix.Extensions;
using System;
using Xunit;

namespace RapideFixFixture.DataTypes
{
  public class SupportedFixVersionTests
  {
    [Fact]
    public void GivenSupportedFixVersion_ToString_ReturnsNotEmpty()
    {
      Assert.NotEmpty(SupportedFixVersion.Fix50.ToString());
      Assert.NotEmpty(SupportedFixVersion.Fix44.ToString());
      Assert.NotEmpty(SupportedFixVersion.Fix43.ToString());
      Assert.NotEmpty(SupportedFixVersion.Fix42.ToString());
    }

    [Fact]
    public void GivenSupportedFixVersionBytes_Parse_ReturnsObject()
    {
      var byteRepresentation = SupportedFixVersion.Fix43.ToString().ToByteValueAndSOH();
      var parsed = SupportedFixVersion.Parse(byteRepresentation.AsSpan());
      Assert.Equal(SupportedFixVersion.Fix43, parsed);
    }

    [Fact]
    public void GivenSupportedFixVersionBytes_TryParse_ReturnsTrue()
    {
      var byteRepresentation = SupportedFixVersion.Fix50.ToString().ToByteValueAndSOH();
      var result = SupportedFixVersion.TryParse(byteRepresentation.AsSpan(), out var parsed);
      Assert.Equal(SupportedFixVersion.Fix50, parsed);
      Assert.True(result);
    }

    [Fact]
    public void GivenInvalidSupportedFixVersionBytes_Parse_ThrowsNotSupportedException()
    {
      var byteRepresentation = SupportedFixVersion.Fix43.ToString().ToByteValueAndSOH();
      Assert.Throws<NotSupportedException>(() => SupportedFixVersion.Parse(byteRepresentation.AsSpan(1)));
    }

    [Fact]
    public void GivenInvalidSupportedFixVersionBytes_TryParse_ReturnsFalse()
    {
      var byteRepresentation = SupportedFixVersion.Fix50.ToString().ToByteValueAndSOH();
      var result = SupportedFixVersion.TryParse(byteRepresentation.AsSpan(1), out var parsed);
      Assert.False(result);
    }

  }
}
