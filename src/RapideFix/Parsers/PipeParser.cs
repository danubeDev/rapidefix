using System;
using System.Buffers;
using System.IO.Pipelines;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RapideFix.Parsers
{
  public class PipeParser<T> : ObservableParser<T>
  {
    private readonly PipeReader _reader;
    private readonly IMessageParser<T, byte> _messageParser;
    private readonly byte[] _fixVersion;
    private readonly Func<ReadOnlyMemory<byte>, T>? _targetObjectFactory;

    public PipeParser(PipeReader pipeReader, IMessageParser<T, byte> singleMessageParser, SupportedFixVersion fixVersion)
       : this(pipeReader, singleMessageParser, fixVersion, null)
    {
    }

    public PipeParser(Pipe pipe, IMessageParser<T, byte> singleMessageParser, SupportedFixVersion fixVersion, Func<ReadOnlyMemory<byte>, T>? targetObjectFactory)
      : this(pipe.Reader, singleMessageParser, fixVersion, targetObjectFactory)
    {
    }

    public PipeParser(PipeReader pipeReader, IMessageParser<T, byte> singleMessageParser, SupportedFixVersion fixVersion, Func<ReadOnlyMemory<byte>, T>? targetObjectFactory)
    {
      _reader = pipeReader ?? throw new ArgumentNullException(nameof(pipeReader));
      _messageParser = singleMessageParser ?? throw new ArgumentNullException(nameof(singleMessageParser));
      _targetObjectFactory = targetObjectFactory;
      _fixVersion = new byte[fixVersion.Value.Length + 2];
      int offset = Encoding.ASCII.GetBytes("8=".AsSpan(), _fixVersion);
      fixVersion.Value.CopyTo(_fixVersion.AsSpan().Slice(offset));
    }

    public override async Task ListenAsync(CancellationToken token)
    {
      while(!token.IsCancellationRequested)
      {
        ReadResult result = await _reader.ReadAsync(token);
        ReadOnlySequence<byte> buffer = result.Buffer;
        SequencePosition? position = PositionOf(buffer.Slice(buffer.GetPosition(1)), _fixVersion);
        while(position != null && !token.IsCancellationRequested)
        {
          ProcessLine(buffer.Slice(0, position.Value));
          // Skip to the next message
          buffer = buffer.Slice(buffer.GetPosition(0, position.Value));
          position = PositionOf(buffer.Slice(1), _fixVersion);
        }

        _reader.AdvanceTo(buffer.Start, buffer.End);
        if(result.IsCompleted)
        {
          if(buffer.Length > 0)
          {
            ProcessLine(buffer);
          }
          break;
        }
      }

      _reader.Complete();
      CompleteObservers();
      token.ThrowIfCancellationRequested();
    }

    private void ProcessLine(ReadOnlySequence<byte> messageSequence)
    {
      if(messageSequence.Length == 0)
      {
        return;
      }
      T parsedObject = default!;
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
