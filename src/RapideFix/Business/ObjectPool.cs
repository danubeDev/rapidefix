using System;
using System.Collections.Concurrent;

namespace RapideFix.Business
{
  public class ObjectPool<T> : IObjectPool<T>
  {
    private readonly ConcurrentBag<PooledObject<T>> _objects;
    private readonly Func<T> _objectGenerator;
    private readonly Action<PooledObject<T>> _cleanUpObject;

    public ObjectPool(Func<T> objectGenerator, Action<PooledObject<T>> cleanUpObject)
    {
      _objectGenerator = objectGenerator ?? throw new ArgumentNullException(nameof(objectGenerator));
      _cleanUpObject = cleanUpObject ?? throw new ArgumentNullException(nameof(cleanUpObject));
      _objects = new ConcurrentBag<PooledObject<T>>();
    }

    public PooledObject<T> Rent()
    {
      PooledObject<T> item;
      if(_objects.TryTake(out item))
      {
        return item;
      }
      return new PooledObject<T>(this, _objectGenerator());
    }

    public void Return(PooledObject<T> item)
    {
      _cleanUpObject(item);
      _objects.Add(item);
    }
  }

}
