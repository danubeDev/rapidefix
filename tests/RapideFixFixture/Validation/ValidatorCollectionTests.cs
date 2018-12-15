using System;
using RapideFix;
using RapideFix.DataTypes;
using RapideFix.Extensions;
using RapideFix.Validation;
using Xunit;

namespace RapideFixFixture.Validation
{
  public class ValidatorCollectionTests
  {
    [Fact]
    public void GivenDependencies_Construct_DoesNotThrow()
    {
      var record = Record.Exception(() => new ValidatorCollection(IntegerToFixConverter.Instance));
      Assert.Null(record);
    }

    [Fact]
    public void GivenMessageContext_IsValid_ReturnsTrue()
    {
      var uut = new ValidatorCollection(IntegerToFixConverter.Instance);
      var message = new TestFixMessageBuilder(TestFixMessageBuilder.DefaultBody).Build(out byte checksum, out var dummy);
      var msgCtx = new FixMessageContext().Setup(message);
      msgCtx.ChecksumValue = checksum;

      Assert.True(uut.PreValidate(message, msgCtx));
      Assert.True(uut.PostValidate(message, msgCtx));
    }

    [Fact]
    public void GivenNoMessageContext_IsValid_ThrowsArgumentNullException()
    {
      var uut = new ValidatorCollection(IntegerToFixConverter.Instance);
      var message = new TestFixMessageBuilder(TestFixMessageBuilder.DefaultBody).Build();
      Assert.Throws<ArgumentNullException>(() => uut.PreValidate(message, null));
    }

  }
}
