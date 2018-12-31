using System;
using System.Globalization;
using RapideFix.Business.Data;
using RapideFix.DataTypes;

namespace RapideFix.Business.PropertySetters
{
  public class LongSetter : SinglePropertySetterBase, ITypedPropertySetter
  {
    private static NumberFormatInfo _numberFormatInfo = CultureInfo.CurrentCulture.NumberFormat;

    public override object Set(ReadOnlySpan<char> valueChars, TagMapLeaf mappingDetails, FixMessageContext fixMessageContext, object targetObject)
    {
      if(long.TryParse(valueChars, NumberStyles.Integer, _numberFormatInfo, out var parsedValue))
      {
        SetValue(mappingDetails, fixMessageContext, targetObject, parsedValue);
      }
      return targetObject;
    }

    public override TTarget SetTarget<TTarget>(ReadOnlySpan<char> valueChars, TagMapLeaf mappingDetails, FixMessageContext fixMessageContext, ref TTarget targetObject)
    {
      if(long.TryParse(valueChars, out var parsedValue))
      {
        SetValue<TTarget, long>(mappingDetails, fixMessageContext, ref targetObject, parsedValue);
      }
      return targetObject;
    }

  }
}