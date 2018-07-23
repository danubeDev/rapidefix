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
    private readonly ConcurrentDictionary<Type, Delegate> _delegateFactoryCache = new ConcurrentDictionary<Type, Delegate>();

    public object Set(Span<byte> value, TagMapLeaf mappingDetails, FixMessageContext fixMessageContext, object targetObject)
    {
      // mappingDetails is a leaf node for the repeating tag
      // parents are expected to be set by parents setter
      if(mappingDetails is RepeatingGroupTagMapLeaf repeatingLeaf)
      {
        targetObject = CreateEnumerable(value, repeatingLeaf, fixMessageContext, targetObject);
      }

      return targetObject;
    }

    public object CreateEnumerable(Span<byte> value, RepeatingGroupTagMapLeaf repeatingLeaf, FixMessageContext fixMessageContext, object targetObject)
    {
      //This is handled as a parent. The incremental counting is set by the first tag of the repeating group.
      if(fixMessageContext.CreatedParentTypes == null || !fixMessageContext.CreatedParentTypes.Contains(GetKey(repeatingLeaf.Current)))
      {
        if(fixMessageContext.CreatedParentTypes == null)
        {
          fixMessageContext.CreatedParentTypes = new HashSet<int>();
        }
        Span<char> valueChars = stackalloc char[value.Length];
        Encoding.ASCII.GetChars(value, valueChars);
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
      targetObject = repeatingLeaf.Current.GetValue(targetObject);
      return targetObject;
    }
  }
}