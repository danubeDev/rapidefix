﻿using System;
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
      var message = new TestFixMessageBuilder(TestFixMessageBuilder.DefaultBody).Build();
      var msgCtx = new FixMessageContext().Setup(message);

      Assert.True(uut.IsValid(message, msgCtx));
    }

    [Fact]
    public void GivenNoMessageContext_IsValid_ThrowsArgumentNullException()
    {
      var uut = new ValidatorCollection(IntegerToFixConverter.Instance);
      var message = new TestFixMessageBuilder(TestFixMessageBuilder.DefaultBody).Build();
      Assert.Throws<ArgumentNullException>(() => uut.IsValid(message, null));
    }

  }
}