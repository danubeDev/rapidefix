using System;
using System.Collections.Concurrent;
using RapideFix.Business.Data;
using RapideFix.DataTypes;

namespace RapideFix.Business
{
  public class ParentTypeSetter : IPropertySetter
  {
    public object Set(ReadOnlySpan<byte> value, TagMapLeaf mappingDetails, FixMessageContext fixMessageContext, object targetObject)
    {
      foreach(var parent in mappingDetails.Parents)
      {
        targetObject = parent.ParentSetter.Set(mappingDetails, parent, fixMessageContext, targetObject);
      }

      return targetObject;
    }

    public object Set(ReadOnlySpan<char> value, TagMapLeaf mappingDetails, FixMessageContext fixMessageContext, object targetObject)
    {
      foreach(var parent in mappingDetails.Parents)
      {
        targetObject = parent.ParentSetter.Set(mappingDetails, parent, fixMessageContext, targetObject);
      }

      return targetObject;
    }
  }
}