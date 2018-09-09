namespace RapideFix.Business
{
  public interface IObjectPool<T>
  {
    /// <summary>
    /// Returned a PooledObject of type T from the pool.
    /// </summary>
    PooledObject<T> Rent();

    /// <summary>
    /// Returns the rented PooledObject of type T to the pool.
    /// </summary>
    void Return(PooledObject<T> item);
  }
}