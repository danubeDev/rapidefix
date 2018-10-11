using System;
using System.IO;
using System.IO.Pipelines;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using RapideFix;
using RapideFix.Parsers;
using RapideFixFixture.Mocks;
using RapideFixFixture.TestTypes;
using Xunit;

namespace RapideFixFixture.Parsers
{
  public class StreamParserTests
  {
    [Fact]
    public void GivenNulls_Construct_ThrowsArgumentNullException()
    {
      Assert.Throws<ArgumentNullException>(() => new StreamParser<TestTypeParent>(null, Mock.Of<IMessageParser<TestTypeParent>>(), SupportedFixVersion.Fix42));
      Assert.Throws<ArgumentNullException>(() => new StreamParser<TestTypeParent>(Mock.Of<Stream>(), null, SupportedFixVersion.Fix42));
    }

    [Fact]
    public void GivenDependencies_Construct_DoesNotThrow()
    {
      var ex = Record.Exception(() => new StreamParser<TestTypeParent>(Mock.Of<Stream>(), Mock.Of<IMessageParser<TestTypeParent>>(), SupportedFixVersion.Fix42));
      Assert.Null(ex);
      ex = Record.Exception(() => new StreamParser<TestTypeParent>(Mock.Of<Stream>(), Mock.Of<IMessageParser<TestTypeParent>>(), SupportedFixVersion.Fix42, null));
      Assert.Null(ex);
    }

    [Fact]
    public async Task GivenStreamedMessage_Observable_PushesParsedMessages()
    {
      using(var stream = new MemoryStream())
      {
        var mockMessageParser = new MockMessageParser();
        int messages = 0;
        var cancellation = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        TaskCompletionSource<bool> taskCompletion = new TaskCompletionSource<bool>();
        byte[] msg = TestFixMessageBuilder.CreateDefaultMessage();
        await stream.WriteAsync(msg);
        stream.Position = 0;

        var uut = new StreamParser<TestTypeParent>(stream, mockMessageParser, SupportedFixVersion.Fix44);

        uut.Subscribe(parsedObject => messages++, ex => taskCompletion.SetResult(false), () => taskCompletion.SetResult(true));
        var listener = Task.Run(() => uut.ListenAsync(cancellation.Token));

        Assert.True(await taskCompletion.Task);
        Assert.Equal(1, messages);
        await listener;
      }
    }

    [Fact]
    public async Task GivenStreamedMessages_Observable_PushesParsedMessages()
    {
      using(var stream = new MemoryStream())
      {
        var mockMessageParser = new MockMessageParser();
        int messages = 0;
        var cancellation = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        TaskCompletionSource<bool> taskCompletion = new TaskCompletionSource<bool>();
        for(int i = 0; i < 10; i++)
        {
          foreach(var sampleParams in SampleFixMessagesSource.GetTestTypeParentMessageBodies())
          {
            byte[] msg = new TestFixMessageBuilder(sampleParams[0] as string).Build();
            await stream.WriteAsync(msg);
          }
        }
        stream.Position = 0;

        var uut = new StreamParser<TestTypeParent>(stream, mockMessageParser, SupportedFixVersion.Fix44);

        uut.Subscribe(parsedObject => messages++, ex => taskCompletion.SetResult(false), () => taskCompletion.SetResult(true));
        var listener = Task.Run(() => uut.ListenAsync(cancellation.Token));

        Assert.True(await taskCompletion.Task);
        Assert.Equal(10 * SampleFixMessagesSource.GetTestTypeParentMessageBodies().Count(), messages);
        await listener;
      }
    }
  }
}
