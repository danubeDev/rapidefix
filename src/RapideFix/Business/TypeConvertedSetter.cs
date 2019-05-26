using System;
using System.Buffers;
using System.ComponentModel;
using System.Reflection;
using RapideFix.Business.Data;
using RapideFix.Business.PropertySetters;
using RapideFix.DataTypes;

namespace RapideFix.Business
{
  public class TypeConvertedSetter : BaseSetter, IPropertySetter
  {
    private TypeConverter? _typeConverter;
    private Delegate? _delegateFactory;

    public override object Set(ReadOnlySpan<char> valueChars, TagMapLeaf mappingDetails, FixMessageContext fixMessageContext, object targetObject)
    {
      if(mappingDetails.TypeConverterName is null)
      {
        return targetObject;
      }
      if(_typeConverter == null)
      {
        Type typeOfConverter = Type.GetType(mappingDetails.TypeConverterName);
        _typeConverter = (TypeConverter)Activator.CreateInstance(typeOfConverter);
      }
      if(!_typeConverter.CanConvertFrom(typeof(char[])))
      {
        return targetObject;
      }
      var tempCharsArray = ArrayPool<char>.Shared.Rent(valueChars.Length);
      try
      {
        valueChars.CopyTo(tempCharsArray.AsSpan());
        object converted = _typeConverter.ConvertFrom(tempCharsArray);
        var convertedType = converted.GetType();
        if(_delegateFactory == null)
        {
          string methodGeneratingMethodName = mappingDetails.IsEnumerable ? "GetEnumeratedILSetterAction" : "GetILSetterAction";

          var methodInfo = typeof(BaseSetter).GetMethod(methodGeneratingMethodName, BindingFlags.NonPublic | BindingFlags.Instance);
          _delegateFactory = (Delegate)methodInfo
            .MakeGenericMethod(convertedType)
            .Invoke(this, new[] { mappingDetails.Current });
        }
        if(mappingDetails.IsEnumerable)
        {
          int index = GetAdvancedIndex(mappingDetails, fixMessageContext);
          _delegateFactory.DynamicInvoke(targetObject, converted, index);
        }
        else
        {
          _delegateFactory.DynamicInvoke(targetObject, converted);
        }
      }
      finally
      {
        ArrayPool<char>.Shared.Return(tempCharsArray, true);
      }

      return targetObject;
    }

    public override TTarget SetTarget<TTarget>(ReadOnlySpan<char> valueChars, TagMapLeaf mappingDetails, FixMessageContext fixMessageContext, ref TTarget targetObject)
    {
      throw new NotSupportedException();
    }
  }
}