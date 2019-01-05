using System;
using RapideFix.Business.Data;
using RapideFix.DataTypes;

namespace RapideFix.Business.PropertySetters
{
  public class CharSetter : BaseSetter, ITypedPropertySetter
  {
    public override object Set(ReadOnlySpan<char> valueChars, TagMapLeaf mappingDetails, FixMessageContext fixMessageContext, object targetObject)
    {
      SetValue(mappingDetails, fixMessageContext, targetObject, valueChars[0]);
      return targetObject;
    }

    public override TTarget SetTarget<TTarget>(ReadOnlySpan<char> valueChars, TagMapLeaf mappingDetails, FixMessageContext fixMessageContext, ref TTarget targetObject)
    {
      SetValue<TTarget, char>(mappingDetails, fixMessageContext, ref targetObject, valueChars[0]);
      return targetObject;
    }

  }
}