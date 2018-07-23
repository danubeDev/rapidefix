using System;
using System.Buffers;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using RapideFix.Business.Data;
using RapideFix.DataTypes;

namespace RapideFix.Business
{
  public class TypeConvertedSetter : BaseTypeSetter, IPropertySetter
  {
    private readonly ConcurrentDictionary<string, TypeConverter> _typeConverters = new ConcurrentDictionary<string, TypeConverter>();
    private readonly ConcurrentDictionary<int, Delegate> _delegateFactory = new ConcurrentDictionary<int, Delegate>();

    public object Set(Span<byte> value, TagMapLeaf mappingDetails, FixMessageContext fixMessageContext, object targetObject)
    {
      if(mappingDetails.TypeConverterName == null)
      {
        return targetObject;
      }
      int valueLength = value.Length;
      Span<char> valueChars = stackalloc char[valueLength];
      valueLength = Decode(value, mappingDetails, fixMessageContext, valueChars);
      valueChars = valueChars.Slice(0, valueLength);

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
        if(!_delegateFactory.TryGetValue(GetKey(mappingDetails.Current), out Delegate delegateMethod))
        {
          string methodGeneratingMethodName = mappingDetails is IEnumerableTag ? "GetEnumeratedILSetterAction" : "GetILSetterAction";

          var methodInfo = typeof(SimpleTypeSetter).GetMethod(methodGeneratingMethodName, BindingFlags.NonPublic | BindingFlags.Instance);
          delegateMethod = (Delegate)methodInfo
            .MakeGenericMethod(convertedType)
            .Invoke(this, new[] { mappingDetails.Current });
          _delegateFactory.TryAdd(GetKey(mappingDetails.Current), delegateMethod);

        }
        if(mappingDetails is EnumerableTagMapLeaf enumerableLeaf)
        {
          int index = GetAdvancedIndex(enumerableLeaf, fixMessageContext);
          delegateMethod.DynamicInvoke(targetObject, converted, index);
        }
        else
        {
          delegateMethod.DynamicInvoke(targetObject, converted);
        }
      }
      finally
      {
        ArrayPool<char>.Shared.Return(tempCharsArray, true);
      }

      return targetObject;
    }

  }
}