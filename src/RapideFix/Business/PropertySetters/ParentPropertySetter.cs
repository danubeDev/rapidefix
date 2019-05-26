using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using RapideFix.Business.Data;
using RapideFix.DataTypes;

namespace RapideFix.Business.PropertySetters
{
  public class ParentPropertySetter : BaseSetter, IParentSetter
  {
    private Delegate? _delegateFactoryCache;

    public override object Set(ReadOnlySpan<char> valueChars, TagMapLeaf mappingDetails, FixMessageContext fixMessageContext, object targetObject)
    {
      throw new NotSupportedException();
    }

    public object Set(TagMapLeaf mappingDetails, TagMapParent parent, FixMessageContext fixMessageContext, object targetObject)
    {
      if(parent.IsEnumerable)
      {
        targetObject = SetRepeatingTypeParent(mappingDetails, parent, fixMessageContext, targetObject);
      }
      else
      {
        targetObject = SetSimpleTypeParent(parent, fixMessageContext, targetObject);
      }
      return targetObject;
    }

    public override TTarget SetTarget<TTarget>(ReadOnlySpan<char> valueChars, TagMapLeaf mappingDetails, FixMessageContext fixMessageContext, ref TTarget targetObject)
    {
      throw new NotSupportedException();
    }

    private object SetRepeatingTypeParent(TagMapLeaf leaf, TagMapParent parent, FixMessageContext fixMessageContext, object targetObject)
    {
      int index = GetAdvancedIndex(GetKey(leaf.Current), parent, fixMessageContext, out bool isAdvanced);

      if(isAdvanced)
      {
        object childObject = Activator.CreateInstance(parent.InnerType);
        Type typeOfParent = parent.InnerType!;
        if(_delegateFactoryCache == null)
        {
          var methodInfo = typeof(BaseSetter).GetMethod("GetEnumeratedILSetterAction", BindingFlags.NonPublic | BindingFlags.Instance);
          _delegateFactoryCache = (Delegate)methodInfo
            .MakeGenericMethod(typeOfParent)
            .Invoke(this, new[] { parent.Current });
        }
        _delegateFactoryCache.DynamicInvoke(targetObject, childObject, index);
        return childObject;
      }

      targetObject = (parent.Current.GetValue(targetObject) as IEnumerable<object>).ElementAt(index);
      return targetObject;
    }

    private object SetSimpleTypeParent(TagMapParent parent, FixMessageContext fixMessageContext, object targetObject)
    {
      if(!fixMessageContext.CreatedParentTypes.Contains(GetKey(parent.Current)))
      {
        object childObject = Activator.CreateInstance(parent.Current.PropertyType);
        Type typeOfParent = childObject.GetType();
        if(_delegateFactoryCache == null)
        {
          var methodInfo = typeof(BaseSetter).GetMethod("GetILSetterAction", BindingFlags.NonPublic | BindingFlags.Instance);
          _delegateFactoryCache = (Delegate)methodInfo
            .MakeGenericMethod(typeOfParent)
            .Invoke(this, new[] { parent.Current });
        }
        _delegateFactoryCache.DynamicInvoke(targetObject, childObject);

        fixMessageContext.CreatedParentTypes.Add(GetKey(parent.Current));
        return childObject;
      }
      targetObject = parent.Current.GetValue(targetObject);
      return targetObject;
    }
  }
}