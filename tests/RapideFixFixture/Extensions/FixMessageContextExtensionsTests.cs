using System;
using System.Collections.Generic;
using RapideFix.DataTypes;
using RapideFix.Extensions;
using Xunit;

namespace RapideFixFixture.Extensions
{
  public class FixMessageContextExtensions
  {
    [Fact]
    public void GivenSampleMessage_Setup_ReturnsMessageContext()
    {
      var message = new TestFixMessageBuilder(TestFixMessageBuilder.DefaultBody).Build();
      var uut = new FixMessageContext();
      var result = uut.Setup(message.AsSpan());

      Assert.NotEqual(-1, result.ChecksumTagStartIndex);
      Assert.NotEqual(-1, result.LengthTagStartIndex);
      Assert.NotEqual(-1, result.MessageTypeTagStartIndex);
    }

    [Fact]
    public void GivenEncoding_Setup_ReturnsMessageContext()
    {
      var message = new TestFixMessageBuilder(TestFixMessageBuilder.DefaultBody).Build();
      var uut = new FixMessageContext();
      var result = uut.Setup(message.AsSpan(), MessageEncoding.JIS);

      Assert.NotEqual(-1, result.ChecksumTagStartIndex);
      Assert.NotEqual(-1, result.LengthTagStartIndex);
      Assert.NotEqual(-1, result.MessageTypeTagStartIndex);
      Assert.Equal(MessageEncoding.JIS, result.EncodedFields);
    }

    [Fact]
    public void GivenMessageContext_SetupTwice_ResetsProperties()
    {
      var message = new TestFixMessageBuilder(TestFixMessageBuilder.DefaultBody).Build();
      var uut = new FixMessageContext();
      uut.CreatedParentTypes.Add(1);
      uut.RepeatingGroupCounters.Add(1, new FixMessageContext.RepeatingCounter(1));
      var result = uut.Setup(message.AsSpan(), MessageEncoding.JIS).Setup(message.AsSpan(), MessageEncoding.EUC);

      Assert.NotEqual(-1, result.ChecksumTagStartIndex);
      Assert.NotEqual(-1, result.LengthTagStartIndex);
      Assert.NotEqual(-1, result.MessageTypeTagStartIndex);
      Assert.Equal(MessageEncoding.EUC, result.EncodedFields);
      Assert.Empty(result.RepeatingGroupCounters);
      Assert.Empty(result.CreatedParentTypes);
    }

  }
}
