using System;
using RapideFix.Business.Data;
using RapideFix.DataTypes;

namespace RapideFix.Business.PropertySetters
{
  public class NullableBooleanSetter : BaseSetter, ITypedPropertySetter
  {
    public override object Set(ReadOnlySpan<char> valueChars, TagMapLeaf mappingDetails, FixMessageContext fixMessageContext, object targetObject)
    {
      if(bool.TryParse(valueChars, out var parsedValue))
      {
        SetValue<bool?>(mappingDetails, fixMessageContext, targetObject, parsedValue);
      }
      else
      {
        if(valueChars[0] == Constants.True || valueChars[0] == Constants.TrueNumber)
        {
          SetValue<bool?>(mappingDetails, fixMessageContext, targetObject, true);
        }
        if(valueChars[0] == Constants.False || valueChars[0] == Constants.FalseNumber)
        {
          SetValue<bool?>(mappingDetails, fixMessageContext, targetObject, false);
        }
      }
      return targetObject;
    }

    public override TTarget SetTarget<TTarget>(ReadOnlySpan<char> valueChars, TagMapLeaf mappingDetails, FixMessageContext fixMessageContext, ref TTarget targetObject)
    {
      if(bool.TryParse(valueChars, out var parsedValue))
      {
        SetValue<TTarget, bool?>(mappingDetails, fixMessageContext, ref targetObject, parsedValue);
      }
      else
      {
        if(valueChars[0] == Constants.True || valueChars[0] == Constants.TrueNumber)
        {
          SetValue<TTarget, bool?>(mappingDetails, fixMessageContext, ref targetObject, true);
        }
        if(valueChars[0] == Constants.False || valueChars[0] == Constants.FalseNumber)
        {
          SetValue<TTarget, bool?>(mappingDetails, fixMessageContext, ref targetObject, false);
        }
      }
      return targetObject;
    }

  }
}