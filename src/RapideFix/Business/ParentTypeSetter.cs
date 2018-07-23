using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using RapideFix.Business.Data;
using RapideFix.DataTypes;

namespace RapideFix.Business
{
  public class ParentTypeSetter : BaseTypeSetter, IPropertySetter
  {
    private readonly ConcurrentDictionary<int, Delegate> _delegateFactoryCache = new ConcurrentDictionary<int, Delegate>();

    public object Set(Span<byte> value, TagMapLeaf mappingDetails, FixMessageContext fixMessageContext, object targetObject)
    {
      foreach(var parent in mappingDetails.Parents)
      {
        if(parent is EnumerableTagMapNode enumerableParent)
        {
          targetObject = SetRepeatingTypeParent(mappingDetails, enumerableParent, fixMessageContext, targetObject);
        }
        else
        {
          targetObject = SetSimpleTypeParent(parent, fixMessageContext, targetObject);
        }
      }

      return targetObject;
    }

    private object SetRepeatingTypeParent(TagMapLeaf leaf, EnumerableTagMapNode parent, FixMessageContext fixMessageContext, object targetObject)
    {
      int index = GetAdvancedIndex(GetKey(leaf.Current), parent, fixMessageContext, out bool isAdvanced);

      if(isAdvanced)
      {
        object childObject = Activator.CreateInstance(parent.InnerType);
        Type typeOfParent = parent.InnerType;
        if(!_delegateFactoryCache.TryGetValue(GetKey(parent.Current), out Delegate delegateMethod))
        {
          var methodInfo = typeof(SimpleTypeSetter).GetMethod("GetEnumeratedILSetterAction", BindingFlags.NonPublic | BindingFlags.Instance);
          delegateMethod = (Delegate)methodInfo
            .MakeGenericMethod(typeOfParent)
            .Invoke(this, new[] { parent.Current });
          _delegateFactoryCache.TryAdd(GetKey(parent.Current), delegateMethod);
        }
        delegateMethod.DynamicInvoke(targetObject, childObject, index);
        return childObject;
      }

      targetObject = (parent.Current.GetValue(targetObject) as IEnumerable<object>).ElementAt(index);
      return targetObject;
    }

    public object SetSimpleTypeParent(TagMapNode parent, FixMessageContext fixMessageContext, object targetObject)
    {
      if(fixMessageContext.CreatedParentTypes == null || !fixMessageContext.CreatedParentTypes.Contains(GetKey(parent.Current)))
      {
        if(fixMessageContext.CreatedParentTypes == null)
        {
          fixMessageContext.CreatedParentTypes = new HashSet<int>();
        }
        object childObject = Activator.CreateInstance(parent.Current.PropertyType);
        Type typeOfParent = childObject.GetType();
        if(!_delegateFactoryCache.TryGetValue(GetKey(parent.Current), out Delegate delegateMethod))
        {
          var methodInfo = typeof(SimpleTypeSetter).GetMethod("GetILSetterAction", BindingFlags.NonPublic | BindingFlags.Instance);
          delegateMethod = (Delegate)methodInfo
            .MakeGenericMethod(typeOfParent)
            .Invoke(this, new[] { parent.Current });
          _delegateFactoryCache.TryAdd(GetKey(parent.Current), delegateMethod);
        }
        delegateMethod.DynamicInvoke(targetObject, childObject);

        fixMessageContext.CreatedParentTypes.Add(GetKey(parent.Current));
        return childObject;
      }
      targetObject = parent.Current.GetValue(targetObject);
      return targetObject;
    }
  }
}