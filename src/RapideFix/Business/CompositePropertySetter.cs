using System;
using RapideFix.Business.Data;
using RapideFix.DataTypes;

namespace RapideFix.Business
{
  public class CompositePropertySetter : ITypedPropertySetter
  {
    public object Set(ReadOnlySpan<byte> value, TagMapLeaf mappingDetails, FixMessageContext fixMessageContext, object targetObject)
    {
      object parentTarget = targetObject;
      if(mappingDetails.Parents != null && mappingDetails.Parents.Count > 0)
      {
        foreach(var parent in mappingDetails.Parents)
        {
          parentTarget = parent.ParentSetter.Set(mappingDetails, parent, fixMessageContext, targetObject);
        }
      }

      parentTarget = mappingDetails.Setter.Set(value, mappingDetails, fixMessageContext, parentTarget);
      return parentTarget;
    }

    public object Set(ReadOnlySpan<char> value, TagMapLeaf mappingDetails, FixMessageContext fixMessageContext, object targetObject)
    {
      object parentTarget = targetObject;
      if(mappingDetails.Parents != null && mappingDetails.Parents.Count > 0)
      {
        foreach(var parent in mappingDetails.Parents)
        {
          parentTarget = parent.ParentSetter.Set(mappingDetails, parent, fixMessageContext, targetObject);
        }
      }

      parentTarget = mappingDetails.Setter.Set(value, mappingDetails, fixMessageContext, parentTarget);
      return parentTarget;
    }

    public TTarget SetTarget<TTarget>(ReadOnlySpan<byte> value, TagMapLeaf mappingDetails, FixMessageContext fixMessageContext, ref TTarget targetObject)
    {
      targetObject = mappingDetails.Setter.SetTarget(value, mappingDetails, fixMessageContext, ref targetObject);
      return targetObject;
    }

    public TTarget SetTarget<TTarget>(ReadOnlySpan<char> value, TagMapLeaf mappingDetails, FixMessageContext fixMessageContext, ref TTarget targetObject)
    {
      targetObject = mappingDetails.Setter.SetTarget(value, mappingDetails, fixMessageContext, ref targetObject);
      return targetObject;
    }
  }
}
