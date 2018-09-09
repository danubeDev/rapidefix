using System;
using Moq;
using RapideFix.Business;
using RapideFix.DataTypes;
using RapideFix.Extensions;
using Xunit;

namespace RapideFixFixture.Extensions
{
  public class PooledObjectFixMessageContextExtensionsTests
  {
    [Fact]
    public void GivenSampleMessage_Init_ReturnsPooledMessageContext()
    {
      var message = new TestFixMessageBuilder(TestFixMessageBuilder.DefaultBody).Build();
      var uut = new PooledObject<FixMessageContext>(Mock.Of<IObjectPool<FixMessageContext>>(), new FixMessageContext());
      var result = uut.Init(message.AsSpan());

      Assert.NotEqual(-1, result.Value.ChecksumTagStartIndex);
      Assert.NotEqual(-1, result.Value.LengthTagStartIndex);
      Assert.NotEqual(-1, result.Value.MessageTypeTagStartIndex);
    }

    [Fact]
    public void GivenEncoding_Init_ReturnsPooledMessageContext()
    {
      var message = new TestFixMessageBuilder(TestFixMessageBuilder.DefaultBody).Build();
      var uut = new PooledObject<FixMessageContext>(Mock.Of<IObjectPool<FixMessageContext>>(), new FixMessageContext());
      var result = uut.Init(message.AsSpan(), MessageEncoding.JIS);

      Assert.NotEqual(-1, result.Value.ChecksumTagStartIndex);
      Assert.NotEqual(-1, result.Value.LengthTagStartIndex);
      Assert.NotEqual(-1, result.Value.MessageTypeTagStartIndex);
      Assert.Equal(MessageEncoding.JIS, result.Value.EncodedFields);
    }

    [Fact]
    public void GivenPooledMessageContext_CleanUp_ResetsProperties()
    {
      var message = new TestFixMessageBuilder(TestFixMessageBuilder.DefaultBody).Build();
      var uut = new PooledObject<FixMessageContext>(Mock.Of<IObjectPool<FixMessageContext>>(), new FixMessageContext());
      var result = uut.Init(message.AsSpan(), MessageEncoding.JIS).Reset();

      Assert.Equal(-1, result.Value.ChecksumTagStartIndex);
      Assert.Equal(-1, result.Value.LengthTagStartIndex);
      Assert.Equal(-1, result.Value.MessageTypeTagStartIndex);
    }

  }
}
