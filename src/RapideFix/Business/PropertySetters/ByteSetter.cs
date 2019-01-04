using System;
using RapideFix.Business.Data;
using RapideFix.DataTypes;

namespace RapideFix.Business.PropertySetters
{
  public class ByteSetter : BaseSetter, ITypedPropertySetter
  {
    public override object Set(ReadOnlySpan<char> valueChars, TagMapLeaf mappingDetails, FixMessageContext fixMessageContext, object targetObject)
    {
      SetValue(mappingDetails, fixMessageContext, targetObject, (byte)valueChars[0]);
      return targetObject;
    }

    public override TTarget SetTarget<TTarget>(ReadOnlySpan<char> valueChars, TagMapLeaf mappingDetails, FixMessageContext fixMessageContext, ref TTarget targetObject)
    {
      SetValue<TTarget, byte>(mappingDetails, fixMessageContext, ref targetObject, (byte)valueChars[0]);
      return targetObject;
    }

  }
}