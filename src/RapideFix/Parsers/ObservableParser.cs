using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO.Pipelines;
using System.Threading;
using System.Threading.Tasks;

namespace RapideFix.Parsers
{
  public abstract class ObservableParser<T> : IObservable<T>
  {
    private readonly List<IObserver<T>> _observers;
    protected IEnumerable<IObserver<T>> Observers => _observers;

    public ObservableParser()
    {
      _observers = new List<IObserver<T>>();
    }

    public IDisposable Subscribe(IObserver<T> observer)
    {
      _observers.Add(observer);
      return new Unsubscriber<T>(_observers, observer);
    }

    public Task ListenAsync()
    {
      return ListenAsync(CancellationToken.None);
    }

    public abstract Task ListenAsync(CancellationToken token);

    protected SequencePosition? PositionOf(in ReadOnlySequence<byte> source, ReadOnlySpan<byte> value)
    {
      if(source.IsSingleSegment)
      {
        int index = source.First.Span.IndexOf(value);
        if(index != -1)
        {
          return source.GetPosition(index);
        }
        return null;
      }
      else
      {
        return PositionOfMultiSegment(source, value);
      }
    }

    protected SequencePosition? PositionOfMultiSegment(in ReadOnlySequence<byte> source, ReadOnlySpan<byte> value)
    {
      SequencePosition position = source.Start;
      SequencePosition result = position;
      while(source.TryGet(ref position, out ReadOnlyMemory<byte> memory))
      {
        int index = memory.Span.IndexOf(value);
        if(index != -1)
        {
          return source.GetPosition(index, result);
        }
        else if(position.GetObject() == null)
        {
          break;
        }
        result = position;
      }
      return null;

    }

    protected void CompleteObservers()
    {
      foreach(var observer in _observers.ToArray())
      {
        if(_observers.Contains(observer))
        {
          observer.OnCompleted();
        }
      }
      _observers.Clear();
    }

    private class Unsubscriber<TData> : IDisposable
    {
      private readonly List<IObserver<TData>> _observers;
      private readonly IObserver<TData> _observer;

      public Unsubscriber(List<IObserver<TData>> observers, IObserver<TData> observer)
      {
        _observers = observers;
        _observer = observer;
      }

      public void Dispose()
      {
        if(_observer != null && _observers.Contains(_observer))
        {
          _observers.Remove(_observer);
        }
      }
    }


  }
}
