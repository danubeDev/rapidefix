using System;
using RapideFix.Business.Data;
using RapideFix.DataTypes;

namespace RapideFix.Business
{
  public class SimpleTypeSetter : BaseTypeSetter, IPropertySetter
  {
    public object Set(Span<byte> value, TagMapLeaf mappingDetails, FixMessageContext fixMessageContext, object targetObject)
    {
      int valueLength = value.Length;
      Span<char> valueChars = stackalloc char[valueLength];
      valueLength = Decode(value, mappingDetails, fixMessageContext, valueChars);
      valueChars = valueChars.Slice(0, valueLength);
      var propertyType = !(mappingDetails is IEnumerableTag enumerableLeaf) ?
        mappingDetails.Current.PropertyType : enumerableLeaf.InnerType;


      if(propertyType == typeof(int))
      {
        if(int.TryParse(valueChars, out var parsedValue))
        {
          SetValue(mappingDetails, fixMessageContext, targetObject, parsedValue);
        }
      }
      else if(propertyType == typeof(double))
      {
        if(double.TryParse(valueChars, out var parsedValue))
        {
          SetValue(mappingDetails, fixMessageContext, targetObject, parsedValue);
        }
      }
      else if(propertyType == typeof(decimal))
      {
        if(decimal.TryParse(valueChars, out var parsedValue))
        {
          SetValue(mappingDetails, fixMessageContext, targetObject, parsedValue);
        }
      }
      else if(propertyType == typeof(long))
      {
        if(long.TryParse(valueChars, out var parsedValue))
        {
          SetValue(mappingDetails, fixMessageContext, targetObject, parsedValue);
        }
      }
      else if(propertyType == typeof(short))
      {
        if(short.TryParse(valueChars, out var parsedValue))
        {
          SetValue(mappingDetails, fixMessageContext, targetObject, parsedValue);
        }
      }
      else if(propertyType == typeof(float))
      {
        if(float.TryParse(valueChars, out var parsedValue))
        {
          SetValue(mappingDetails, fixMessageContext, targetObject, parsedValue);
        }
      }
      else if(propertyType == typeof(bool))
      {
        if(bool.TryParse(valueChars, out var parsedValue))
        {
          SetValue(mappingDetails, fixMessageContext, targetObject, parsedValue);
        }
        else
        {
          if(valueChars[0] == Constants.True || valueChars[0] == Constants.TrueNumber)
          {
            SetValue(mappingDetails, fixMessageContext, targetObject, true);
          }
          if(valueChars[0] == Constants.False || valueChars[0] == Constants.FalseNumber)
          {
            SetValue(mappingDetails, fixMessageContext, targetObject, false);
          }
        }
      }
      else if(propertyType == typeof(byte))
      {
        SetValue(mappingDetails, fixMessageContext, targetObject, value[0]);
      }
      else if(propertyType == typeof(char))
      {
        SetValue(mappingDetails, fixMessageContext, targetObject, valueChars[0]);
      }
      else if(propertyType == typeof(string))
      {
        SetValue(mappingDetails, fixMessageContext, targetObject, valueChars.ToString());
      }
      if(propertyType == typeof(int?))
      {
        if(int.TryParse(valueChars, out var parsedValue))
        {
          SetValue<int?>(mappingDetails, fixMessageContext, targetObject, parsedValue);
        }
      }
      else if(propertyType == typeof(double?))
      {
        if(double.TryParse(valueChars, out var parsedValue))
        {
          SetValue<double?>(mappingDetails, fixMessageContext, targetObject, parsedValue);
        }
      }
      else if(propertyType == typeof(decimal?))
      {
        if(decimal.TryParse(valueChars, out var parsedValue))
        {
          SetValue<decimal?>(mappingDetails, fixMessageContext, targetObject, parsedValue);
        }
      }
      else if(propertyType == typeof(long?))
      {
        if(long.TryParse(valueChars, out var parsedValue))
        {
          SetValue<long?>(mappingDetails, fixMessageContext, targetObject, parsedValue);
        }
      }
      else if(propertyType == typeof(short?))
      {
        if(short.TryParse(valueChars, out var parsedValue))
        {
          SetValue<short?>(mappingDetails, fixMessageContext, targetObject, parsedValue);
        }
      }
      else if(propertyType == typeof(float?))
      {
        if(float.TryParse(valueChars, out var parsedValue))
        {
          SetValue<float?>(mappingDetails, fixMessageContext, targetObject, parsedValue);
        }
      }
      else if(propertyType == typeof(bool?))
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
      }
      else if(propertyType == typeof(byte?))
      {
        SetValue<byte?>(mappingDetails, fixMessageContext, targetObject, value[0]);
      }
      else if(propertyType == typeof(char?))
      {
        SetValue<char?>(mappingDetails, fixMessageContext, targetObject, valueChars[0]);
      }

      return targetObject;
    }

  }
}