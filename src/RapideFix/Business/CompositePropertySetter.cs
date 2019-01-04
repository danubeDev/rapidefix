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

      if(mappingDetails.IsRepeatingGroupTag)
      {
        parentTarget = mappingDetails.Setter.Set(value, mappingDetails, fixMessageContext, parentTarget);
      }
      else if(mappingDetails.Setter != null)
      {
        parentTarget = mappingDetails.Setter.Set(value, mappingDetails, fixMessageContext, parentTarget);
      }
      else if(mappingDetails.TypeConverterName != null)
      {
        parentTarget = mappingDetails.Setter.Set(value, mappingDetails, fixMessageContext, parentTarget);
      }
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

      if(mappingDetails.IsRepeatingGroupTag)
      {
        parentTarget = mappingDetails.Setter.Set(value, mappingDetails, fixMessageContext, parentTarget);
      }
      else if(mappingDetails.Setter != null)
      {
        parentTarget = mappingDetails.Setter.Set(value, mappingDetails, fixMessageContext, parentTarget);
      }
      else if(mappingDetails.TypeConverterName != null)
      {
        parentTarget = mappingDetails.Setter.Set(value, mappingDetails, fixMessageContext, parentTarget);
      }
      return parentTarget;
    }

    public TTarget SetTarget<TTarget>(ReadOnlySpan<byte> value, TagMapLeaf mappingDetails, FixMessageContext fixMessageContext, ref TTarget targetObject)
    {
      if(mappingDetails.Parents != null && mappingDetails.Parents.Count > 0)
      {
        throw new NotSupportedException("Typed setting may only work on flat objects");
      }

      if(mappingDetails.IsRepeatingGroupTag)
      {
        mappingDetails.Setter.Set(value, mappingDetails, fixMessageContext, targetObject);
      }
      else if(mappingDetails.Setter != null)
      {
        targetObject = mappingDetails.Setter.SetTarget(value, mappingDetails, fixMessageContext, ref targetObject);
      }
      else if(mappingDetails.TypeConverterName != null)
      {
        throw new NotSupportedException("Typed setting may only work on flat objects");
      }
      return targetObject;
    }

    public TTarget SetTarget<TTarget>(ReadOnlySpan<char> value, TagMapLeaf mappingDetails, FixMessageContext fixMessageContext, ref TTarget targetObject)
    {
      if(mappingDetails.Parents != null && mappingDetails.Parents.Count > 0)
      {
        throw new NotSupportedException("Typed setting may only work on flat objects");
      }

      if(mappingDetails.IsRepeatingGroupTag)
      {
        mappingDetails.Setter.Set(value, mappingDetails, fixMessageContext, targetObject);
      }
      else if(mappingDetails.Setter != null)
      {
        targetObject = mappingDetails.Setter.SetTarget(value, mappingDetails, fixMessageContext, ref targetObject);
      }
      else if(mappingDetails.TypeConverterName != null)
      {
        throw new NotSupportedException("Typed setting may only work on flat objects");
      }
      return targetObject;
    }
  }
}
