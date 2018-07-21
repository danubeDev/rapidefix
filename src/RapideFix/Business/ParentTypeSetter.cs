using System;
using System.Collections.Generic;
using System.Reflection;
using RapideFix.DataTypes;
using static RapideFix.Business.TagToPropertyMapper;

namespace RapideFix.Business
{
  public class ParentTypeSetter : SimpleTypeSetter
  {
    private readonly Dictionary<Type, Delegate> _delegateFactory = new Dictionary<Type, Delegate>();

    public new object Set(Span<byte> value, TagMapLeaf mappingDetails, FixMessageContext fixMessageContext, object targetObject)
    {
      foreach(var parent in mappingDetails.Parents)
      {
        if(!parent.IsRepeating)
        {
          targetObject = SetSimpleTypeParent(parent, fixMessageContext, targetObject);
        }
      }

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
        object parentObject = Activator.CreateInstance(parent.Current.PropertyType);
        Type typeOfParent = parentObject.GetType();
        if(!_delegateFactory.TryGetValue(typeOfParent, out Delegate delegateMethod))
        {
          var methodInfo = typeof(SimpleTypeSetter).GetMethod("GetILSetterAction", BindingFlags.NonPublic | BindingFlags.Instance);
          delegateMethod = (Delegate)methodInfo
            .MakeGenericMethod(typeOfParent)
            .Invoke(this, new[] { parent.Current });
          _delegateFactory.TryAdd(typeOfParent, delegateMethod);
        }
        delegateMethod.DynamicInvoke(targetObject, parentObject);

        fixMessageContext.CreatedParentTypes.Add(GetKey(parent.Current));
        return parentObject;
      }
      targetObject = parent.Current.GetValue(targetObject);
      return targetObject;
    }
  }
}