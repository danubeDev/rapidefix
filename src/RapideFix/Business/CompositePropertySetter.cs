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
      switch(mappingDetails)
      {
        case RepeatingGroupTagMapLeaf repeatingParent:
          parentTarget = _repeatingGroupSetter.Set(value, mappingDetails, fixMessageContext, parentTarget);
          break;
        case TagMapLeaf simpleOrEnumerated when simpleOrEnumerated.TypeConverterName == null:
          parentTarget = _simpleTypeSetter.Set(value, mappingDetails, fixMessageContext, parentTarget);
          break;
        case TagMapLeaf withTypeSetter when withTypeSetter.TypeConverterName != null:
          parentTarget = _typeConvertedSetter.Set(value, mappingDetails, fixMessageContext, parentTarget);
          break;
      }

      return parentTarget;
    }

    public TTarget SetTarget<TTarget>(ReadOnlySpan<byte> value, TagMapLeaf mappingDetails, FixMessageContext fixMessageContext, ref TTarget targetObject)
    {
      if(mappingDetails.Parents != null && mappingDetails.Parents.Count > 0)
      {
        throw new NotSupportedException("Typed setting may only work on flat objects");
      }
      switch(mappingDetails)
      {
        case RepeatingGroupTagMapLeaf repeatingParent:
          _repeatingGroupSetter.Set(value, mappingDetails, fixMessageContext, targetObject);
          break;
        case TagMapLeaf simpleOrEnumerated when simpleOrEnumerated.TypeConverterName == null:
          targetObject = _typedPropertySetter.SetTarget(value, mappingDetails, fixMessageContext, ref targetObject);
          break;
        case TagMapLeaf withTypeSetter when withTypeSetter.TypeConverterName != null:
          throw new NotSupportedException("Typed setting may only work on flat objects");
          break;
      }

      return targetObject;
    }
  }
}
