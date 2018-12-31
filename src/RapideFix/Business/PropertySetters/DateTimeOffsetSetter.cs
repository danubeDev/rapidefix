using System;
using System.Globalization;
using RapideFix.Business.Data;
using RapideFix.DataTypes;

namespace RapideFix.Business.PropertySetters
{
  public class DateTimeOffsetSetter : SinglePropertySetterBase, ITypedPropertySetter
  {
    private const string DateTimeFormat = "yyyyMMdd-HH:mm:ss";

    public override object Set(ReadOnlySpan<char> valueChars, TagMapLeaf mappingDetails, FixMessageContext fixMessageContext, object targetObject)
    {
      if(DateTimeOffset.TryParseExact(valueChars, DateTimeFormat, DateTimeFormatInfo.InvariantInfo, DateTimeStyles.AssumeUniversal, out var parsedValue))
      {
        SetValue<DateTimeOffset>(mappingDetails, fixMessageContext, targetObject, parsedValue);
      }
      return targetObject;
    }

    public override TTarget SetTarget<TTarget>(ReadOnlySpan<char> valueChars, TagMapLeaf mappingDetails, FixMessageContext fixMessageContext, ref TTarget targetObject)
    {
      if(DateTimeOffset.TryParseExact(valueChars, DateTimeFormat, DateTimeFormatInfo.InvariantInfo, DateTimeStyles.AssumeUniversal, out var parsedValue))
      {
        SetValue<TTarget, DateTimeOffset>(mappingDetails, fixMessageContext, ref targetObject, parsedValue);
      }
      return targetObject;
    }

  }
}