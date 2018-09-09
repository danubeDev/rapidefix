using System;

namespace RapideFix.Business
{
  public class PooledObject<T> : IDisposable
  {
    private readonly IObjectPool<T> _pool;
    public PooledObject(IObjectPool<T> pool, T value)
    {
      _pool = pool ?? throw new ArgumentNullException(nameof(pool));
      Value = value;
    }

    public T Value { get; }

    public void Dispose()
    {
      _pool.Return(this);
    }

  }

}
