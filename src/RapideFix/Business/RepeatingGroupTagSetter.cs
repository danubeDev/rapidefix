using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using RapideFix.Business.Data;
using RapideFix.DataTypes;

namespace RapideFix.Business
{
  /// <summary>
  /// Creates arrays for the repeatable types
  /// </summary>
  public class RepeatingGroupTagSetter : BaseTypeSetter, IPropertySetter
  {
    private readonly Dictionary<Type, Delegate> _delegateFactoryCache = new Dictionary<Type, Delegate>();

    public object Set(ReadOnlySpan<byte> value, TagMapLeaf mappingDetails, FixMessageContext fixMessageContext, object targetObject)
    {
      // mappingDetails is a leaf node for the repeating tag
      // parents are expected to be set by parents setter
      if(mappingDetails.IsRepeatingGroupTag)
      {
        //This is handled as a parent. The incremental counting is set by the first tag of the repeating group.
        if(fixMessageContext.CreatedParentTypes is null || !fixMessageContext.CreatedParentTypes.Contains(GetKey(mappingDetails.Current)))
        {
          if(fixMessageContext.CreatedParentTypes is null)
          {
            fixMessageContext.CreatedParentTypes = new HashSet<int>();
          }
          targetObject = CreateEnumerable(value, mappingDetails, fixMessageContext, targetObject);
        }
        else
        {
          targetObject = mappingDetails.Current.GetValue(targetObject);
        }
        return targetObject;
      }

      return targetObject;
    }

    private object CreateEnumerable(ReadOnlySpan<byte> value, TagMapLeaf repeatingLeaf, FixMessageContext fixMessageContext, object targetObject)
    {
      Span<char> valueChars = stackalloc char[value.Length];
      Encoding.ASCII.GetChars(value, valueChars);
      return CreateEnumerable(valueChars, repeatingLeaf, fixMessageContext, targetObject);
    }

    public object Set(ReadOnlySpan<char> value, TagMapLeaf mappingDetails, FixMessageContext fixMessageContext, object targetObject)
    {
      // mappingDetails is a leaf node for the repeating tag
      // parents are expected to be set by parents setter
      if(mappingDetails.IsRepeatingGroupTag)
      {
        //This is handled as a parent. The incremental counting is set by the first tag of the repeating group.
        if(fixMessageContext.CreatedParentTypes is null || !fixMessageContext.CreatedParentTypes.Contains(GetKey(mappingDetails.Current)))
        {
          if(fixMessageContext.CreatedParentTypes is null)
          {
            fixMessageContext.CreatedParentTypes = new HashSet<int>();
          }
          targetObject = CreateEnumerable(value, mappingDetails, fixMessageContext, targetObject);
        }
        else
        {
          targetObject = mappingDetails.Current.GetValue(targetObject);
        }
        return targetObject;
      }

      return targetObject;
    }

    private object CreateEnumerable(ReadOnlySpan<char> valueChars, TagMapLeaf repeatingLeaf, FixMessageContext fixMessageContext, object targetObject)
    {
      if(!int.TryParse(valueChars, out int numberOfItems))
      {
        return targetObject;
      }

      var createdEnumeration = Array.CreateInstance(repeatingLeaf.InnerType, numberOfItems);

      Type typeOfParent = createdEnumeration.GetType();
      if(!_delegateFactoryCache.TryGetValue(typeOfParent, out Delegate delegateMethod))
      {
        var methodInfo = typeof(SimpleTypeSetter).GetMethod("GetILSetterAction", BindingFlags.NonPublic | BindingFlags.Instance);
        delegateMethod = (Delegate)methodInfo
          .MakeGenericMethod(typeOfParent)
          .Invoke(this, new[] { repeatingLeaf.Current });
        _delegateFactoryCache.TryAdd(typeOfParent, delegateMethod);
      }
      delegateMethod.DynamicInvoke(targetObject, createdEnumeration);

      fixMessageContext.CreatedParentTypes.Add(GetKey(repeatingLeaf.Current));
      return createdEnumeration;
    }
  }
}