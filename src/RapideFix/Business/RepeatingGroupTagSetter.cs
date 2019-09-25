using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
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
    private Delegate? _delegateFactoryCache;

    public override object Set(ReadOnlySpan<char> value, TagMapLeaf mappingDetails, FixMessageContext fixMessageContext, object targetObject)
    {
      // mappingDetails is a leaf node for the repeating tag
      // parents are expected to be set by parents setter
      //This is handled as a parent. The incremental counting is set by the first tag of the repeating group.
      return SetInternal(value, mappingDetails, fixMessageContext, targetObject);
    }

    [return: NotNull]
    private object? SetInternal(ReadOnlySpan<char> value, TagMapLeaf mappingDetails, FixMessageContext fixMessageContext, object targetObject)
    {
      if (!fixMessageContext.CreatedParentTypes.Contains(GetKey(mappingDetails.Current)))
        return CreateEnumerable(value, mappingDetails, fixMessageContext, targetObject);
      else
        return mappingDetails.Current.GetValue(targetObject);
    }

    private object CreateEnumerable(ReadOnlySpan<char> valueChars, TagMapLeaf repeatingLeaf, FixMessageContext fixMessageContext, object targetObject)
    {
      if (!int.TryParse(valueChars, out int numberOfItems))
        return targetObject;

      var createdEnumeration = Array.CreateInstance(repeatingLeaf.InnerType, numberOfItems);

      Type typeOfParent = createdEnumeration.GetType();
      if (_delegateFactoryCache == null)
      {
        var methodInfo = typeof(BaseSetter).GetMethod("GetILSetterAction", BindingFlags.NonPublic | BindingFlags.Instance);
        var generatedDelegate = (Delegate?)methodInfo?.MakeGenericMethod(typeOfParent).Invoke(this, new[] { repeatingLeaf.Current });
        _delegateFactoryCache = generatedDelegate ?? throw new InvalidOperationException();
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