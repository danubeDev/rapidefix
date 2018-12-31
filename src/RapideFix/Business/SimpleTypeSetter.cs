using System;
using System.Globalization;
using System.Text;
using RapideFix.Business.Data;
using RapideFix.DataTypes;

namespace RapideFix.Business
{
  public class SimpleTypeSetter : BaseTypeSetter, ITypedPropertySetter
  {
    private static NumberFormatInfo _numberFormatInfo = CultureInfo.CurrentCulture.NumberFormat;

    public object Set(ReadOnlySpan<byte> value, TagMapLeaf mappingDetails, FixMessageContext fixMessageContext, object targetObject)
    {
      int valueLength = value.Length;
      Span<char> valueChars = stackalloc char[valueLength];
      if(mappingDetails.IsEncoded)
      {
        valueLength = fixMessageContext.EncodedFields.GetEncoder().GetChars(value, valueChars);
        valueChars = valueChars.Slice(0, valueLength);
      }
      else
      {
        valueLength = Encoding.ASCII.GetChars(value, valueChars);
      }
      return Set(valueChars, mappingDetails, fixMessageContext, targetObject);
    }

    public TTarget SetTarget<TTarget>(ReadOnlySpan<byte> value, TagMapLeaf mappingDetails, FixMessageContext fixMessageContext, ref TTarget targetObject)
    {
      int valueLength = value.Length;
      Span<char> valueChars = stackalloc char[valueLength];
      if(mappingDetails.IsEncoded)
      {
        valueLength = fixMessageContext.EncodedFields.GetEncoder().GetChars(value, valueChars);
        valueChars = valueChars.Slice(0, valueLength);
      }
      else
      {
        valueLength = Encoding.ASCII.GetChars(value, valueChars);
      }
      return SetTarget<TTarget>(valueChars, mappingDetails, fixMessageContext, ref targetObject);
    }

    public object Set(ReadOnlySpan<char> valueChars, TagMapLeaf mappingDetails, FixMessageContext fixMessageContext, object targetObject)
    {
      mappingDetails.Setter.Set(valueChars, mappingDetails, fixMessageContext, targetObject);
      return targetObject;
    }

    public TTarget SetTarget<TTarget>(ReadOnlySpan<char> valueChars, TagMapLeaf mappingDetails, FixMessageContext fixMessageContext, ref TTarget targetObject)
    {
      mappingDetails.Setter.SetTarget<TTarget>(valueChars, mappingDetails, fixMessageContext, ref targetObject);
      return targetObject;
    }

  }
}