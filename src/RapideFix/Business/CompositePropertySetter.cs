using System;
using RapideFix.Business.Data;
using RapideFix.DataTypes;

namespace RapideFix.Business
{
  public class CompositePropertySetter : IPropertySetter
  {
    private readonly IPropertySetter _parentSetter;
    private readonly IPropertySetter _simpleTypeSetter;
    private readonly IPropertySetter _typeConvertedSetter;
    private readonly IPropertySetter _repeatingGroupSetter;

    public CompositePropertySetter(ISubPropertySetterFactory propertySetterFactory)
    {
      if(propertySetterFactory == null)
      {
        throw new ArgumentNullException(nameof(propertySetterFactory));
      }
      _parentSetter = propertySetterFactory.GetParentPropertySetter();
      _simpleTypeSetter = propertySetterFactory.GetSimplePropertySetter();
      _typeConvertedSetter = propertySetterFactory.GetTypeConvertedPropertySetter();
      _repeatingGroupSetter = propertySetterFactory.GetRepeatingGroupTagPropertySetter();
    }

    public object Set(Span<byte> value, TagMapLeaf mappingDetails, FixMessageContext fixMessageContext, object targetObject)
    {
      object parentTarget = targetObject;
      if(mappingDetails.Parents != null)
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


  }
}
