using System;
using RapideFix.Business.Data;

namespace RapideFix.Business
{
  public interface ITagToPropertyMapper
  {
    TagMapLeaf Get(byte[] tag);
    void Map(Type type);
    void Map<T>();
  }
}