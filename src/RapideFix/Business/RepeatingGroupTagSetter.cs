using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using RapideFix.Business.Data;
using RapideFix.Business.PropertySetters;
using RapideFix.DataTypes;

namespace RapideFix.Business
{
  /// <summary>
  /// Creates arrays for the repeatable types
  /// </summary>
  public class RepeatingGroupTagSetter : BaseSetter, IPropertySetter
  {
    private Delegate _delegateFactoryCache;

    public override object Set(ReadOnlySpan<char> value, TagMapLeaf mappingDetails, FixMessageContext fixMessageContext, object targetObject)
    {
      // mappingDetails is a leaf node for the repeating tag
      // parents are expected to be set by parents setter
      //This is handled as a parent. The incremental counting is set by the first tag of the repeating group.
      if(!fixMessageContext.CreatedParentTypes.Contains(GetKey(mappingDetails.Current)))
      {
        targetObject = CreateEnumerable(value, mappingDetails, fixMessageContext, targetObject);
      }
      else
      {
        targetObject = mappingDetails.Current.GetValue(targetObject);
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
      if(_delegateFactoryCache == null)
      {
        var methodInfo = typeof(BaseSetter).GetMethod("GetILSetterAction", BindingFlags.NonPublic | BindingFlags.Instance);
        _delegateFactoryCache = (Delegate)methodInfo
          .MakeGenericMethod(typeOfParent)
          .Invoke(this, new[] { repeatingLeaf.Current });
      }
      _delegateFactoryCache.DynamicInvoke(targetObject, createdEnumeration);

      fixMessageContext.CreatedParentTypes.Add(GetKey(repeatingLeaf.Current));
      return createdEnumeration;
    }

    public override TTarget SetTarget<TTarget>(ReadOnlySpan<char> valueChars, TagMapLeaf mappingDetails, FixMessageContext fixMessageContext, ref TTarget targetObject)
    {
      throw new NotSupportedException();
    }
  }
}