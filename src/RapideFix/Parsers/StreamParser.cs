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
    private readonly PipeWriter _pipeWriter;

    public StreamParser(Stream stream, IMessageParser<T> singleMessageParser, SupportedFixVersion fixVersion)
       : this(stream, singleMessageParser, fixVersion, null)
    {
    }

    public StreamParser(Stream stream, IMessageParser<T> singleMessageParser, SupportedFixVersion fixVersion, Func<ReadOnlyMemory<byte>, T> targetObjectFactory)
      : base(new Pipe(), singleMessageParser, fixVersion, targetObjectFactory)
    {
      _stream = stream ?? throw new ArgumentNullException(nameof(stream));
      _pipeWriter = base.Pipe.Writer;
    }

    public override async Task ListenAsync(CancellationToken token)
    {
      var pipeReaderTask = Task.Run(() => base.ListenAsync(token));

      const int minimumBufferSize = 1024;
      while(!token.IsCancellationRequested)
      {
        Memory<byte> memory = _pipeWriter.GetMemory(minimumBufferSize);
        int bytesRead = await _stream.ReadAsync(memory, token);
        if(bytesRead == 0)
        {
          break;
        }
        _pipeWriter.Advance(bytesRead);

        FlushResult result = await _pipeWriter.FlushAsync();
        if(result.IsCompleted || !_stream.CanRead)
        {
          break;
        }
      }

      _pipeWriter.Complete();
      await pipeReaderTask;
      token.ThrowIfCancellationRequested();
    }
  }
}
