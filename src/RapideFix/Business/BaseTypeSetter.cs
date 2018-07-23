using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using RapideFix.Business.Data;
using RapideFix.DataTypes;

namespace RapideFix.Business
{
  public abstract class BaseTypeSetter
  {
    protected readonly ConcurrentDictionary<int, Delegate> _propertySetters = new ConcurrentDictionary<int, Delegate>();

    /// <summary>
    /// Using custom message encoding, decodes the parsed value. Returns the length of the decoded value
    /// </summary>
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
      return valueLength;
    }

    /// <summary>
    /// Returns an action to set a given typed value on a given property
    /// </summary>
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

    /// <summary>
    /// Returns an action to set a given typed value at a given index of an array
    /// </summary>
    protected Action<object, TypeOfProperty, int> GetEnumeratedILSetterAction<TypeOfProperty>(PropertyInfo property)
    {
      Delegate generatedDelegate;
      if(!_propertySetters.TryGetValue(GetKey(property), out generatedDelegate))
      {
        var sourceType = typeof(TypeOfProperty);
        var dynamicMethod = new DynamicMethod("SetArrayIndexFor" + property.DeclaringType.AssemblyQualifiedName + property.Name, null, new[] { typeof(object), sourceType, typeof(int) }, typeof(SimpleTypeSetter));
        var ilGenerator = dynamicMethod.GetILGenerator();
        ilGenerator.Emit(OpCodes.Ldarg_0);
        ilGenerator.Emit(OpCodes.Call, property.GetMethod);
        ilGenerator.Emit(OpCodes.Ldarg_2);
        ilGenerator.Emit(OpCodes.Ldarg_1);
        ilGenerator.Emit(OpCodes.Stelem_I);
        ilGenerator.Emit(OpCodes.Ret);
        generatedDelegate = dynamicMethod.CreateDelegate(typeof(Action<object, TypeOfProperty, int>));
        _propertySetters.TryAdd(GetKey(property), generatedDelegate);
      }
      return (Action<object, TypeOfProperty, int>)generatedDelegate;
    }

    /// <summary>
    /// Sets a typed value on an array property or a non-enumerable property based on the mapping details. 
    /// Advances the index for array prorperties
    /// </summary>
    protected void SetValue<T>(TagMapLeaf mappingDetails, FixMessageContext fixMessageContext, object targetObject, T parsedValue)
    {
      if(mappingDetails is EnumerableTagMapLeaf enumerableTagLeaf)
      {
        int index = GetAdvancedIndex(enumerableTagLeaf, fixMessageContext);
        GetEnumeratedILSetterAction<T>(mappingDetails.Current).Invoke(targetObject, parsedValue, index);
      }
      else
      {
        GetILSetterAction<T>(mappingDetails.Current).Invoke(targetObject, parsedValue);
      }
    }

    /// <summary>
    /// Advances an index for array properties current value
    /// </summary>
    protected int GetAdvancedIndex(EnumerableTagMapLeaf mappingDetails, FixMessageContext fixMessageContext)
    {
      return GetAdvancedIndex(GetKey(mappingDetails.Current), mappingDetails, fixMessageContext, out var dummy);
    }

    /// <summary>
    /// Advances an index for array properties. It uses the <paramref name="leafPropertyKey"/> to map a key
    /// for the delimiter tag
    /// </summary>
    protected int GetAdvancedIndex(int leafPropertyKey, IEnumerableTag mappingDetails, FixMessageContext fixMessageContext, out bool isAdvanced)
    {
      if(fixMessageContext.RepeatingGroupCounters == null)
      {
        fixMessageContext.RepeatingGroupCounters = new Dictionary<int, FixMessageContext.RepeatingCounter>();
      }
      isAdvanced = false;
      if(!fixMessageContext.RepeatingGroupCounters.TryGetValue(mappingDetails.RepeatingTagNumber, out FixMessageContext.RepeatingCounter value))
      {
        value = new FixMessageContext.RepeatingCounter(leafPropertyKey);
        fixMessageContext.RepeatingGroupCounters.Add(mappingDetails.RepeatingTagNumber, value);
        isAdvanced = true;
      }
      if(leafPropertyKey == value.FirstTagKey)
      {
        value.Index++;
        isAdvanced = true;
      }
      return value.Index;
    }

    /// <summary>
    /// Generates a has key for a given property info
    /// </summary>
    protected int GetKey(PropertyInfo property)
    {
      return Tuple.Create(property.DeclaringType.AssemblyQualifiedName, property.Name).GetHashCode();
    }
  }
}