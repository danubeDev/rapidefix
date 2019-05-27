using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Text;
using RapideFix.Business.Data;
using RapideFix.DataTypes;

namespace RapideFix.Business.PropertySetters
{
  public abstract class BaseSetter
  {
    protected delegate void ActionRef<TTarget, TValue0>(ref TTarget target, TValue0 value0);

    protected delegate void ActionRef<TTarget, TValue0, TValue1>(ref TTarget target, TValue0 value0, TValue1 value1);

    protected Delegate? _propertySetter;

    /// <summary>
    /// Returns an action to set a given typed value on a given property
    /// </summary>
    protected Action<object, TypeOfProperty> GetILSetterAction<TypeOfProperty>(PropertyInfo property)
    {
      if(_propertySetter == null)
      {
        var sourceType = typeof(TypeOfProperty);
        if(!property.PropertyType.IsAssignableFrom(sourceType))
        {
          throw new ArgumentException($"Property {property.Name}'s type is not assignable from type {sourceType.Name}");
        }
        var dynamicMethod = new DynamicMethod("SetValueFor" + property.DeclaringType.FullName + property.Name, null, new[] { typeof(object), sourceType }, typeof(BaseSetter));
        var ilGenerator = dynamicMethod.GetILGenerator();
        ilGenerator.Emit(OpCodes.Ldarg_0);
        ilGenerator.Emit(OpCodes.Ldarg_1);
        ilGenerator.Emit(OpCodes.Call, property.SetMethod);
        ilGenerator.Emit(OpCodes.Ret);
        _propertySetter = dynamicMethod.CreateDelegate(typeof(Action<object, TypeOfProperty>));
      }
      return (Action<object, TypeOfProperty>)_propertySetter;
    }

    /// <summary>
    /// Returns an action to set a given typed value on a given property on object of type TTarget.
    /// </summary>
    protected ActionRef<TTarget, TypeOfProperty> GetTypedILSetterAction<TTarget, TypeOfProperty>(PropertyInfo property)
    {
      if(_propertySetter == null)
      {
        var sourceType = typeof(TypeOfProperty);
        if(!property.PropertyType.IsAssignableFrom(sourceType))
        {
          throw new ArgumentException($"Property {property.Name}'s type is not assignable from type {sourceType.Name}");
        }
        var dynamicMethod = new DynamicMethod("SetTypedValueFor" + property.DeclaringType.FullName + property.Name, null, new[] { typeof(TTarget).MakeByRefType(), sourceType }, typeof(BaseSetter));
        var ilGenerator = dynamicMethod.GetILGenerator();
        ilGenerator.Emit(OpCodes.Ldarg_0);
        ilGenerator.Emit(OpCodes.Ldarg_1);
        ilGenerator.Emit(OpCodes.Call, property.SetMethod);
        ilGenerator.Emit(OpCodes.Ret);
        _propertySetter = dynamicMethod.CreateDelegate(typeof(ActionRef<TTarget, TypeOfProperty>));
      }
      return (ActionRef<TTarget, TypeOfProperty>)_propertySetter;
    }

    /// <summary>
    /// Returns an action to set a given typed value at a given index of an array
    /// </summary>
    protected Action<object, TypeOfProperty, int> GetEnumeratedILSetterAction<TypeOfProperty>(PropertyInfo property)
    {
      if(_propertySetter == null)
      {
        var sourceType = typeof(TypeOfProperty);
        var dynamicMethod = new DynamicMethod("SetArrayIndexFor" + property.DeclaringType.FullName + property.Name, null, new[] { typeof(object), sourceType, typeof(int) }, typeof(BaseSetter));
        var ilGenerator = dynamicMethod.GetILGenerator();
        ilGenerator.Emit(OpCodes.Ldarg_0);
        ilGenerator.Emit(OpCodes.Call, property.GetMethod);
        ilGenerator.Emit(OpCodes.Ldarg_2);
        ilGenerator.Emit(OpCodes.Ldarg_1);
        ilGenerator.Emit(OpCodes.Stelem_I);
        ilGenerator.Emit(OpCodes.Ret);
        _propertySetter = dynamicMethod.CreateDelegate(typeof(Action<object, TypeOfProperty, int>));
      }
      return (Action<object, TypeOfProperty, int>)_propertySetter;
    }

    /// <summary>
    /// Returns an action to set a given typed value at a given index of an array
    /// </summary>
    protected ActionRef<TTarget, TypeOfProperty, int> GetTypedEnumeratedILSetterAction<TTarget, TypeOfProperty>(PropertyInfo property)
    {
      if(_propertySetter == null)
      {
        var sourceType = typeof(TypeOfProperty);
        var dynamicMethod = new DynamicMethod("SetArrayIndexFor" + property.DeclaringType.FullName + property.Name, null, new[] { typeof(TTarget).MakeByRefType(), sourceType, typeof(int) }, typeof(BaseSetter));
        var ilGenerator = dynamicMethod.GetILGenerator();
        ilGenerator.Emit(OpCodes.Ldarg_0);
        ilGenerator.Emit(OpCodes.Call, property.GetMethod);
        ilGenerator.Emit(OpCodes.Ldarg_2);
        ilGenerator.Emit(OpCodes.Ldarg_1);
        ilGenerator.Emit(OpCodes.Stelem_I);
        ilGenerator.Emit(OpCodes.Ret);
        _propertySetter = dynamicMethod.CreateDelegate(typeof(ActionRef<TTarget, TypeOfProperty, int>));
      }
      return (ActionRef<TTarget, TypeOfProperty, int>)_propertySetter;
    }

    /// <summary>
    /// Sets a typed value on an array property or a non-enumerable property based on the mapping details. 
    /// Advances the index for array prorperties
    /// </summary>
    protected void SetValue<T>(TagMapLeaf mappingDetails, FixMessageContext fixMessageContext, object targetObject, T parsedValue)
    {
      if(mappingDetails.IsEnumerable)
      {
        int index = GetAdvancedIndex(mappingDetails, fixMessageContext);
        GetEnumeratedILSetterAction<T>(mappingDetails.Current).Invoke(targetObject, parsedValue, index);
      }
      else
      {
        GetILSetterAction<T>(mappingDetails.Current).Invoke(targetObject, parsedValue);
      }
    }

    /// <summary>
    /// Sets a typed value on an array property or a non-enumerable property based on the mapping details. 
    /// </summary>
    protected void SetValue<TTarget, T>(TagMapLeaf mappingDetails, FixMessageContext fixMessageContext, ref TTarget targetObject, T parsedValue)
    {
      if(mappingDetails.IsEnumerable)
      {
        int index = GetAdvancedIndex(mappingDetails, fixMessageContext);
        GetTypedEnumeratedILSetterAction<TTarget, T>(mappingDetails.Current).Invoke(ref targetObject, parsedValue, index);
      }
      else
      {
        GetTypedILSetterAction<TTarget, T>(mappingDetails.Current).Invoke(ref targetObject, parsedValue);
      }
    }

    /// <summary>
    /// Advances an index for array properties current value
    /// </summary>
    protected int GetAdvancedIndex(TagMapLeaf mappingDetails, FixMessageContext fixMessageContext)
    {
      return GetAdvancedIndex(GetKey(mappingDetails.Current), mappingDetails, fixMessageContext, out var dummy);
    }

    /// <summary>
    /// Advances an index for array properties. It uses the <paramref name="leafPropertyKey"/> to map a key
    /// for the delimiter tag
    /// </summary>
    protected int GetAdvancedIndex(int leafPropertyKey, TagMapNode mappingDetails, FixMessageContext fixMessageContext, out bool isAdvanced)
    {
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
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected int GetKey(PropertyInfo property)
    {
      return property.GetHashCode();
    }

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

    public abstract object Set(ReadOnlySpan<char> valueChars, TagMapLeaf mappingDetails, FixMessageContext fixMessageContext, object targetObject);

    public abstract TTarget SetTarget<TTarget>(ReadOnlySpan<char> valueChars, TagMapLeaf mappingDetails, FixMessageContext fixMessageContext, ref TTarget targetObject);
  }
}