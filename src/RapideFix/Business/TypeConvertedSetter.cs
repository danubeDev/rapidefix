using System;
using System.Buffers;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using RapideFix.DataTypes;
using static RapideFix.Business.TagToPropertyMapper;

namespace RapideFix.Business
{
  public class TypeConvertedSetter : SimpleTypeSetter
  {
    private readonly Dictionary<string, TypeConverter> _typeConverters = new Dictionary<string, TypeConverter>();
    private readonly Dictionary<Type, Delegate> _delegateFactory = new Dictionary<Type, Delegate>();

    public new object Set(Span<byte> value, TagMapLeaf mappingDetails, FixMessageContext fixMessageContext, object targetObject)
    {
      if(mappingDetails.TypeConverterName == null)
      {
        return targetObject;
      }
      int valueLength = value.Length;
      Span<char> valueChars = stackalloc char[valueLength];
      valueLength = Decode(value, mappingDetails, fixMessageContext, valueChars);

      TypeConverter converter;
      if(!_typeConverters.TryGetValue(mappingDetails.TypeConverterName, out converter))
      {
        Type typeOfConverter = Type.GetType(mappingDetails.TypeConverterName);
        converter = (TypeConverter)Activator.CreateInstance(typeOfConverter);
        _typeConverters.TryAdd(mappingDetails.TypeConverterName, converter);
      }
      if(!converter.CanConvertFrom(typeof(char[])))
      {
        return targetObject;
      }
      var tempCharsArray = ArrayPool<char>.Shared.Rent(valueLength);
      try
      {
        valueChars.CopyTo(tempCharsArray.AsSpan());
        object converted = converter.ConvertFrom(tempCharsArray);
        var convertedType = converted.GetType();
        if(!_delegateFactory.TryGetValue(convertedType, out Delegate delegateMethod))
        {
          var methodInfo = typeof(SimpleTypeSetter).GetMethod("GetILSetterAction", BindingFlags.NonPublic | BindingFlags.Instance);
          delegateMethod = (Delegate)methodInfo
            .MakeGenericMethod(convertedType)
            .Invoke(this, new[] { mappingDetails.Current });
          _delegateFactory.TryAdd(convertedType, delegateMethod);
        }

        delegateMethod.DynamicInvoke(targetObject, converted);
      }
      finally
      {
        ArrayPool<char>.Shared.Return(tempCharsArray);
      }

      return targetObject;
    }

  }
}