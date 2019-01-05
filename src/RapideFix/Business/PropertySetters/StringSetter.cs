using System;
using RapideFix.Business.Data;
using RapideFix.DataTypes;

namespace RapideFix.Business.PropertySetters
{
  public class StringSetter : BaseSetter, ITypedPropertySetter
  {
    public override object Set(ReadOnlySpan<char> valueChars, TagMapLeaf mappingDetails, FixMessageContext fixMessageContext, object targetObject)
    {
      SetValue(mappingDetails, fixMessageContext, targetObject, new string(valueChars));
      return targetObject;
    }

    public override TTarget SetTarget<TTarget>(ReadOnlySpan<char> valueChars, TagMapLeaf mappingDetails, FixMessageContext fixMessageContext, ref TTarget targetObject)
    {
      SetValue<TTarget, string>(mappingDetails, fixMessageContext, ref targetObject, new string(valueChars));
      return targetObject;
    }

  }
}