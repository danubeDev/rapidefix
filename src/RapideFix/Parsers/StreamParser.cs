using System;
using System.IO;
using System.IO.Pipelines;
using System.Threading;
using System.Threading.Tasks;

namespace RapideFix.Parsers
{
  public class StreamParser<T> : PipeParser<T>
  {
    private readonly Stream _stream;
    private readonly Pipe _pipe;

    public StreamParser(Stream stream, IMessageParser<T, byte> singleMessageParser, SupportedFixVersion fixVersion)
       : this(stream, singleMessageParser, fixVersion, null)
    {
    }

    public StreamParser(Stream stream, IMessageParser<T, byte> singleMessageParser, SupportedFixVersion fixVersion, Func<ReadOnlyMemory<byte>, T>? targetObjectFactory)
      : this(new Pipe(), stream, singleMessageParser, fixVersion, targetObjectFactory)
    {
    }

    protected StreamParser(Pipe pipe, Stream stream, IMessageParser<T, byte> singleMessageParser, SupportedFixVersion fixVersion, Func<ReadOnlyMemory<byte>, T>? targetObjectFactory)
     : base(pipe.Reader, singleMessageParser, fixVersion, targetObjectFactory)
    {
      _stream = stream ?? throw new ArgumentNullException(nameof(stream));
      _pipe = pipe ?? throw new ArgumentNullException(nameof(pipe)); 
    }

    public override async Task ListenAsync(CancellationToken token)
    {
      var pipeReaderTask = Task.Run(() => base.ListenAsync(token));
      var pipeWriter = _pipe.Writer;
      const int minimumBufferSize = 1024;
      while (!token.IsCancellationRequested)
      {
        Memory<byte> memory = pipeWriter.GetMemory(minimumBufferSize);
        int bytesRead = await _stream.ReadAsync(memory, token);
        if (bytesRead == 0)
        {
          break;
        }
        pipeWriter.Advance(bytesRead);

        FlushResult result = await pipeWriter.FlushAsync();
        if (result.IsCompleted || !_stream.CanRead)
        {
          break;
        }
      }

      pipeWriter.Complete();
      await pipeReaderTask;
      token.ThrowIfCancellationRequested();
    }
  }
}
