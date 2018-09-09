using System;
using RapideFix.Business;
using RapideFix.DataTypes;

namespace RapideFix.Extensions
{
  public static class PooledObjectExtensions
  {
    public static PooledObject<FixMessageContext> Init(this PooledObject<FixMessageContext> pooledObject, ReadOnlySpan<byte> data)
    {
      pooledObject.Value.Setup(data);
      return pooledObject;
    }

    public static PooledObject<FixMessageContext> Init(
      this PooledObject<FixMessageContext> pooledObject,
      ReadOnlySpan<byte> data,
      MessageEncoding encoding)
    {
      pooledObject.Value.Setup(data, encoding);
      return pooledObject;
    }

    public static PooledObject<FixMessageContext> Reset(this PooledObject<FixMessageContext> pooledObject)
    {
      pooledObject.Value.CleanUp();
      return pooledObject;
    }

  }
}
