using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using RapideFix.DataTypes;
using static RapideFix.Business.TagToPropertyMapper;

namespace RapideFix.Business
{
  public class SimpleTypeSetter
  {
    protected readonly Dictionary<int, Delegate> _propertySetters = new Dictionary<int, Delegate>();

    public object Set(Span<byte> value, TagMapLeaf mappingDetails, FixMessageContext fixMessageContext, object targetObject)
    {
      int valueLength = value.Length;
      Span<char> valueChars = stackalloc char[valueLength];
      valueLength = Decode(value, mappingDetails, fixMessageContext, valueChars);

      if(mappingDetails.Current.PropertyType == typeof(int))
      {
        if(int.TryParse(valueChars, out var parsedValue))
        {
          GetILSetterAction<int>(mappingDetails.Current).Invoke(targetObject, parsedValue);
        }
      }
      else if(mappingDetails.Current.PropertyType == typeof(double))
      {
        if(double.TryParse(valueChars, out var parsedValue))
        {
          GetILSetterAction<double>(mappingDetails.Current).Invoke(targetObject, parsedValue);
        }
      }
      else if(mappingDetails.Current.PropertyType == typeof(decimal))
      {
        if(decimal.TryParse(valueChars, out var parsedValue))
        {
          GetILSetterAction<decimal>(mappingDetails.Current).Invoke(targetObject, parsedValue);
        }
      }
      else if(mappingDetails.Current.PropertyType == typeof(long))
      {
        if(long.TryParse(valueChars, out var parsedValue))
        {
          GetILSetterAction<long>(mappingDetails.Current).Invoke(targetObject, parsedValue);
        }
      }
      else if(mappingDetails.Current.PropertyType == typeof(short))
      {
        if(short.TryParse(valueChars, out var parsedValue))
        {
          GetILSetterAction<short>(mappingDetails.Current).Invoke(targetObject, parsedValue);
        }
      }
      else if(mappingDetails.Current.PropertyType == typeof(float))
      {
        if(float.TryParse(valueChars, out var parsedValue))
        {
          GetILSetterAction<float>(mappingDetails.Current).Invoke(targetObject, parsedValue);
        }
      }
      else if(mappingDetails.Current.PropertyType == typeof(bool))
      {
        if(bool.TryParse(valueChars, out var parsedValue))
        {
          GetILSetterAction<bool>(mappingDetails.Current).Invoke(targetObject, parsedValue);
        }
        else
        {
          if(valueChars[0] == Constants.True || valueChars[0] == Constants.TrueNumber)
          {
            GetILSetterAction<bool>(mappingDetails.Current).Invoke(targetObject, true);
          }
          if(valueChars[0] == Constants.False || valueChars[0] == Constants.FalseNumber)
          {
            GetILSetterAction<bool>(mappingDetails.Current).Invoke(targetObject, true);
          }
        }
      }
      else if(mappingDetails.Current.PropertyType == typeof(byte))
      {
        GetILSetterAction<byte>(mappingDetails.Current).Invoke(targetObject, value[0]);
      }
      else if(mappingDetails.Current.PropertyType == typeof(char))
      {
        GetILSetterAction<char>(mappingDetails.Current).Invoke(targetObject, valueChars[0]);
      }
      else if(mappingDetails.Current.PropertyType == typeof(string))
      {
        GetILSetterAction<string>(mappingDetails.Current).Invoke(targetObject, valueChars.ToString());
      }
      if(mappingDetails.Current.PropertyType == typeof(int?))
      {
        if(int.TryParse(valueChars, out var parsedValue))
        {
          GetILSetterAction<int?>(mappingDetails.Current).Invoke(targetObject, parsedValue);
        }
      }
      else if(mappingDetails.Current.PropertyType == typeof(double?))
      {
        if(double.TryParse(valueChars, out var parsedValue))
        {
          GetILSetterAction<double?>(mappingDetails.Current).Invoke(targetObject, parsedValue);
        }
      }
      else if(mappingDetails.Current.PropertyType == typeof(decimal?))
      {
        if(decimal.TryParse(valueChars, out var parsedValue))
        {
          GetILSetterAction<decimal?>(mappingDetails.Current).Invoke(targetObject, parsedValue);
        }
      }
      else if(mappingDetails.Current.PropertyType == typeof(long?))
      {
        if(long.TryParse(valueChars, out var parsedValue))
        {
          GetILSetterAction<long?>(mappingDetails.Current).Invoke(targetObject, parsedValue);
        }
      }
      else if(mappingDetails.Current.PropertyType == typeof(short?))
      {
        if(short.TryParse(valueChars, out var parsedValue))
        {
          GetILSetterAction<short?>(mappingDetails.Current).Invoke(targetObject, parsedValue);
        }
      }
      else if(mappingDetails.Current.PropertyType == typeof(float?))
      {
        if(float.TryParse(valueChars, out var parsedValue))
        {
          GetILSetterAction<float?>(mappingDetails.Current).Invoke(targetObject, parsedValue);
        }
      }
      else if(mappingDetails.Current.PropertyType == typeof(bool?))
      {
        if(bool.TryParse(valueChars, out var parsedValue))
        {
          GetILSetterAction<bool?>(mappingDetails.Current).Invoke(targetObject, parsedValue);
        }
        else
        {
          if(valueChars[0] == Constants.True || valueChars[0] == Constants.TrueNumber)
          {
            GetILSetterAction<bool?>(mappingDetails.Current).Invoke(targetObject, true);
          }
          if(valueChars[0] == Constants.False || valueChars[0] == Constants.FalseNumber)
          {
            GetILSetterAction<bool?>(mappingDetails.Current).Invoke(targetObject, true);
          }
        }
      }
      else if(mappingDetails.Current.PropertyType == typeof(byte?))
      {
        GetILSetterAction<byte?>(mappingDetails.Current).Invoke(targetObject, value[0]);
      }
      else if(mappingDetails.Current.PropertyType == typeof(char?))
      {
        GetILSetterAction<char?>(mappingDetails.Current).Invoke(targetObject, valueChars[0]);
      }

      return targetObject;
    }

    protected int Decode(Span<byte> value, TagMapLeaf mappingDetails, FixMessageContext fixMessageContext, Span<char> result)
    {
      int valueLength;
      if(mappingDetails.IsEncoded)
      {
        valueLength = fixMessageContext.EncodedFields.GetEncoder().GetChars(value, result);
      }
      else
      {
        valueLength = Encoding.ASCII.GetChars(value, result);
      }
      result = result.Slice(0, valueLength);
      return valueLength;
    }

    protected Action<object, TypeOfProperty> GetILSetterAction<TypeOfProperty>(PropertyInfo property)
    {
      Delegate generatedDelegate;
      if(!_propertySetters.TryGetValue(GetKey(property), out generatedDelegate))
      {
        var sourceType = typeof(TypeOfProperty);
        if(!property.PropertyType.IsAssignableFrom(sourceType))
        {
          throw new ArgumentException($"Property {property.Name}'s type is not assignable from type {sourceType.Name}");
        }
        var dynamicMethod = new DynamicMethod("SetValueFor" + property.DeclaringType.AssemblyQualifiedName + property.Name, null, new[] { typeof(object), sourceType }, typeof(SimpleTypeSetter));
        var ilGenerator = dynamicMethod.GetILGenerator();
        ilGenerator.Emit(OpCodes.Ldarg_0);
        ilGenerator.Emit(OpCodes.Ldarg_1);
        ilGenerator.Emit(OpCodes.Call, property.SetMethod);
        ilGenerator.Emit(OpCodes.Ret);
        generatedDelegate = dynamicMethod.CreateDelegate(typeof(Action<object, TypeOfProperty>));
        _propertySetters.TryAdd(GetKey(property), generatedDelegate);
      }
      return (Action<object, TypeOfProperty>)generatedDelegate;
    }

    protected int GetKey(PropertyInfo property)
    {
      return Tuple.Create(property.DeclaringType.AssemblyQualifiedName, property.Name).GetHashCode();
    }
  }
}