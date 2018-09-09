using System;
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
  public class PipeParserTests
  {
    [Fact]
    public void GivenNulls_Construct_ThrowsArgumentNullException()
    {
      Assert.Throws<ArgumentNullException>(() => new PipeParser<TestTypeParent>(null, Mock.Of<IMessageParser<TestTypeParent>>(), SupportedFixVersion.Fix42));
      Assert.Throws<ArgumentNullException>(() => new PipeParser<TestTypeParent>(Mock.Of<PipeReader>(), null, SupportedFixVersion.Fix42));
    }

    [Fact]
    public void GivenDependencies_Construct_DoesNotThrow()
    {
      var ex = Record.Exception(() => new PipeParser<TestTypeParent>(Mock.Of<PipeReader>(), Mock.Of<IMessageParser<TestTypeParent>>(), SupportedFixVersion.Fix42));
      Assert.Null(ex);
      ex = Record.Exception(() => new PipeParser<TestTypeParent>(Mock.Of<PipeReader>(), Mock.Of<IMessageParser<TestTypeParent>>(), SupportedFixVersion.Fix42, null));
      Assert.Null(ex);
      ex = Record.Exception(() => new PipeParser<TestTypeParent>(new Pipe(), Mock.Of<IMessageParser<TestTypeParent>>(), SupportedFixVersion.Fix42, null));
      Assert.Null(ex);
    }

    [Fact]
    public async Task GivenPipedMessage_Observable_PushesParsedMessages()
    {
      var pipe = new Pipe();
      var mockMessageParser = new MockMessageParser();
      int messages = 0;
      var cancellation = new CancellationTokenSource(TimeSpan.FromSeconds(5));
      TaskCompletionSource<bool> taskCompletion = new TaskCompletionSource<bool>();

      var uut = new PipeParser<TestTypeParent>(pipe.Reader, mockMessageParser, SupportedFixVersion.Fix42);

      uut.Subscribe(parsedObject => messages++, ex => taskCompletion.SetResult(false), () => taskCompletion.SetResult(true));
      var listener = Task.Run(() => uut.ListenAsync(cancellation.Token));

      byte[] msg = TestFixMessageBuilder.CreateDefaultMessage();
      await pipe.Writer.WriteAsync(msg);
      await pipe.Writer.FlushAsync();
      pipe.Writer.Complete();

      Assert.True(await taskCompletion.Task);
      Assert.Equal(1, messages);
      await listener;
    }

    [Fact]
    public async Task GivenPipedMessages_Observable_PushesParsedMessages()
    {
      var pipe = new Pipe();
      var mockMessageParser = new MockMessageParser();
      int messages = 0;
      var cancellation = new CancellationTokenSource(TimeSpan.FromSeconds(5));
      TaskCompletionSource<bool> taskCompletion = new TaskCompletionSource<bool>();

      var uut = new PipeParser<TestTypeParent>(pipe.Reader, mockMessageParser, SupportedFixVersion.Fix42);

      uut.Subscribe(parsedObject => messages++, ex => taskCompletion.SetResult(false), () => taskCompletion.SetResult(true));
      var listener = Task.Run(() => uut.ListenAsync(cancellation.Token));

      // 10 loops to make sure we have more message then the default buffer can hold.
      for(int i = 0; i < 10; i++)
      {
        foreach(var sampleParams in SampleFixMessagesSource.GetTestTypeParentMessageBodies())
        {
          byte[] msg = new TestFixMessageBuilder(sampleParams[0] as string).Build();
          await pipe.Writer.WriteAsync(msg);
        }
      }
      await pipe.Writer.FlushAsync();
      pipe.Writer.Complete();

      Assert.True(await taskCompletion.Task);
      Assert.Equal(10 * SampleFixMessagesSource.GetTestTypeParentMessageBodies().Count(), messages);
      await listener;
    }

  }
}
