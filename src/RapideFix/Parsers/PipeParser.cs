using System;
using System.Buffers;
using System.IO.Pipelines;
using System.Threading;
using System.Threading.Tasks;

namespace RapideFix.Parsers
{
  public class PipeParser<T> : ObservableParser<T>
  {
    private readonly PipeReader _reader;
    private readonly IMessageParser<T, byte> _messageParser;
    private readonly SupportedFixVersion _fixVersion;
    private readonly Func<ReadOnlyMemory<byte>, T> _targetObjectFactory;

    public PipeParser(PipeReader pipeReader, IMessageParser<T, byte> singleMessageParser, SupportedFixVersion fixVersion)
       : this(pipeReader, singleMessageParser, fixVersion, null)
    {
    }

    public PipeParser(Pipe pipe, IMessageParser<T, byte> singleMessageParser, SupportedFixVersion fixVersion, Func<ReadOnlyMemory<byte>, T> targetObjectFactory)
      : this(pipe.Reader, singleMessageParser, fixVersion, targetObjectFactory)
    {
      Pipe = pipe ?? throw new ArgumentNullException(nameof(pipe));
    }

    public PipeParser(PipeReader pipeReader, IMessageParser<T, byte> singleMessageParser, SupportedFixVersion fixVersion, Func<ReadOnlyMemory<byte>, T> targetObjectFactory)
    {
      _reader = pipeReader ?? throw new ArgumentNullException(nameof(pipeReader));
      _messageParser = singleMessageParser ?? throw new ArgumentNullException(nameof(singleMessageParser));
      _targetObjectFactory = targetObjectFactory;
      _fixVersion = fixVersion;
    }

    protected Pipe Pipe { get; }

    public override async Task ListenAsync(CancellationToken token)
    {
      while(!token.IsCancellationRequested)
      {
        ReadResult result = await _reader.ReadAsync(token);
        ReadOnlySequence<byte> buffer = result.Buffer;
        SequencePosition? position = null;
        do
        {
          position = PositionOf(buffer, _fixVersion.Value);
          if(position != null)
          {
            ProcessLine(buffer.Slice(0, position.Value));
            // Skip to the next message
            buffer = buffer.Slice(buffer.GetPosition(1, position.Value));
          }
        }
        while(position != null && !token.IsCancellationRequested);

        _reader.AdvanceTo(buffer.Start, buffer.End);
        if(result.IsCompleted)
        {
          break;
        }
      }

      _reader.Complete();
      CompleteObservers();
      token.ThrowIfCancellationRequested();
    }

    private void ProcessLine(ReadOnlySequence<byte> messageSequence)
    {
      T parsedObject = default;
      try
      {
        if(_targetObjectFactory == null && messageSequence.IsSingleSegment)
        {
          parsedObject = _messageParser.Parse(messageSequence.First.Span);
        }
        else if(_targetObjectFactory == null && !messageSequence.IsSingleSegment)
        {
          parsedObject = _messageParser.Parse(messageSequence.ToArray());
        }
        else if(_targetObjectFactory != null && messageSequence.IsSingleSegment)
        {
          parsedObject = _messageParser.Parse(messageSequence.First, _targetObjectFactory);
        }
        else if(_targetObjectFactory != null && !messageSequence.IsSingleSegment)
        {
          parsedObject = _messageParser.Parse(messageSequence.ToArray(), _targetObjectFactory);
        }
      }
      catch(Exception ex)
      {
        foreach(var observer in Observers)
        {
          observer.OnError(ex);
        }
        return;
      }
      foreach(var observer in Observers)
      {
        observer.OnNext(parsedObject);
      }

    }
  }
}
