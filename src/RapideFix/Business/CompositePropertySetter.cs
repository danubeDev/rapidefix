using System;
using RapideFix.Business.Data;
using RapideFix.DataTypes;

namespace RapideFix.Business
{
  public class CompositePropertySetter : ITypedPropertySetter
  {
    private readonly IPropertySetter _parentSetter;
    private readonly IPropertySetter _simpleTypeSetter;
    private readonly IPropertySetter _typeConvertedSetter;
    private readonly IPropertySetter _repeatingGroupSetter;
    private readonly ITypedPropertySetter _typedPropertySetter;

    public CompositePropertySetter(ISubPropertySetterFactory propertySetterFactory)
    {
      if(propertySetterFactory is null)
      {
        throw new ArgumentNullException(nameof(propertySetterFactory));
      }
      _parentSetter = propertySetterFactory.GetParentPropertySetter();
      _simpleTypeSetter = propertySetterFactory.GetSimplePropertySetter();
      _typeConvertedSetter = propertySetterFactory.GetTypeConvertedPropertySetter();
      _repeatingGroupSetter = propertySetterFactory.GetRepeatingGroupTagPropertySetter();
      _typedPropertySetter = propertySetterFactory.GetTypedPropertySetter();
    }

    public object Set(ReadOnlySpan<byte> value, TagMapLeaf mappingDetails, FixMessageContext fixMessageContext, object targetObject)
    {
      object parentTarget = targetObject;
      if(mappingDetails.Parents != null && mappingDetails.Parents.Count > 0)
      {
        parentTarget = _parentSetter.Set(value, mappingDetails, fixMessageContext, parentTarget);
      }

      if(mappingDetails.IsRepeatingGroupTag)
      {
        parentTarget = _repeatingGroupSetter.Set(value, mappingDetails, fixMessageContext, parentTarget);
      }
      else if(mappingDetails.Setter != null)
      {
        parentTarget = _simpleTypeSetter.Set(value, mappingDetails, fixMessageContext, parentTarget);
      }
      else if(mappingDetails.TypeConverterName != null)
      {
        parentTarget = _typeConvertedSetter.Set(value, mappingDetails, fixMessageContext, parentTarget);
      }
      return parentTarget;
    }

    public object Set(ReadOnlySpan<char> value, TagMapLeaf mappingDetails, FixMessageContext fixMessageContext, object targetObject)
    {
      object parentTarget = targetObject;
      if(mappingDetails.Parents != null && mappingDetails.Parents.Count > 0)
      {
        parentTarget = _parentSetter.Set(value, mappingDetails, fixMessageContext, parentTarget);
      }

      if(mappingDetails.IsRepeatingGroupTag)
      {
        parentTarget = _repeatingGroupSetter.Set(value, mappingDetails, fixMessageContext, parentTarget);
      }
      else if(mappingDetails.Setter != null)
      {
        parentTarget = _simpleTypeSetter.Set(value, mappingDetails, fixMessageContext, parentTarget);
      }
      else if(mappingDetails.TypeConverterName != null)
      {
        parentTarget = _typeConvertedSetter.Set(value, mappingDetails, fixMessageContext, parentTarget);
      }
      return parentTarget;
    }

    public TTarget SetTarget<TTarget>(ReadOnlySpan<byte> value, TagMapLeaf mappingDetails, FixMessageContext fixMessageContext, ref TTarget targetObject)
    {
      if(mappingDetails.Parents != null && mappingDetails.Parents.Count > 0)
      {
        throw new NotSupportedException("Typed setting may only work on flat objects");
      }

      if(mappingDetails.IsRepeatingGroupTag)
      {
        _repeatingGroupSetter.Set(value, mappingDetails, fixMessageContext, targetObject);
      }
      else if(mappingDetails.Setter != null)
      {
        targetObject = _typedPropertySetter.SetTarget(value, mappingDetails, fixMessageContext, ref targetObject);
      }
      else if(mappingDetails.TypeConverterName != null)
      {
        throw new NotSupportedException("Typed setting may only work on flat objects");
      }
      return targetObject;
    }

    public TTarget SetTarget<TTarget>(ReadOnlySpan<char> value, TagMapLeaf mappingDetails, FixMessageContext fixMessageContext, ref TTarget targetObject)
    {
      if(mappingDetails.Parents != null && mappingDetails.Parents.Count > 0)
      {
        throw new NotSupportedException("Typed setting may only work on flat objects");
      }

      if(mappingDetails.IsRepeatingGroupTag)
      {
        _repeatingGroupSetter.Set(value, mappingDetails, fixMessageContext, targetObject);
      }
      else if(mappingDetails.Setter != null)
      {
        targetObject = _typedPropertySetter.SetTarget(value, mappingDetails, fixMessageContext, ref targetObject);
      }
      else if(mappingDetails.TypeConverterName != null)
      {
        throw new NotSupportedException("Typed setting may only work on flat objects");
      }
      return targetObject;
    }
  }
}
