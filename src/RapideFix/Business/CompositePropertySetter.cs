using System;
using System.Text;
using RapideFix.Business.Data;
using RapideFix.DataTypes;

namespace RapideFix.Business
{
  public class CompositePropertySetter : ITypedPropertySetter
  {
    public object Set(ReadOnlySpan<byte> value, TagMapLeaf mappingDetails, FixMessageContext fixMessageContext, object targetObject)
    {
      if(mappingDetails.Parents != null)
      {
        foreach(var parent in mappingDetails.Parents)
        {
          targetObject = parent.ParentSetter.Set(mappingDetails, parent, fixMessageContext, targetObject);
        }
      }

      int valueLength = value.Length;
      Span<char> valueChars = stackalloc char[valueLength];
      if(mappingDetails.IsEncoded)
      {
        valueLength = fixMessageContext.EncodedFields!.GetEncoder().GetChars(value, valueChars);
        valueChars = valueChars.Slice(0, valueLength);
      }
      else
      {
        Encoding.ASCII.GetChars(value, valueChars);
      }

      targetObject = mappingDetails.Setter.Set(valueChars, mappingDetails, fixMessageContext, targetObject);
      return targetObject;
    }

    public object Set(ReadOnlySpan<char> value, TagMapLeaf mappingDetails, FixMessageContext fixMessageContext, object targetObject)
    {
      if(mappingDetails.Parents != null)
      {
        foreach(var parent in mappingDetails.Parents)
        {
          targetObject = parent.ParentSetter.Set(mappingDetails, parent, fixMessageContext, targetObject);
        }
      }

      targetObject = mappingDetails.Setter.Set(value, mappingDetails, fixMessageContext, targetObject);
      return targetObject;
    }

    public TTarget SetTarget<TTarget>(ReadOnlySpan<byte> value, TagMapLeaf mappingDetails, FixMessageContext fixMessageContext, ref TTarget targetObject)
    {
      int valueLength = value.Length;
      Span<char> valueChars = stackalloc char[valueLength];
      if(mappingDetails.IsEncoded)
      {
        valueLength = fixMessageContext.EncodedFields!.GetEncoder().GetChars(value, valueChars);
        valueChars = valueChars.Slice(0, valueLength);
      }
      else
      {
        Encoding.ASCII.GetChars(value, valueChars);
      }
      targetObject = mappingDetails.Setter.SetTarget(valueChars, mappingDetails, fixMessageContext, ref targetObject);
      return targetObject;
    }

    public TTarget SetTarget<TTarget>(ReadOnlySpan<char> value, TagMapLeaf mappingDetails, FixMessageContext fixMessageContext, ref TTarget targetObject)
    {
      targetObject = mappingDetails.Setter.SetTarget(value, mappingDetails, fixMessageContext, ref targetObject);
      return targetObject;
    }
  }
}
