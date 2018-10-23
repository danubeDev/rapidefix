using System;
using RapideFix.Business.Data;
using RapideFix.DataTypes;

namespace RapideFix.Business
{
  public interface ITypedPropertySetter : IPropertySetter
  {
    /// <summary>
    /// Sets the given value on the referenced target object's property mapped by mapping details.
    /// </summary>
    TTarget SetTarget<TTarget>(ReadOnlySpan<byte> value, TagMapLeaf mappingDetails, FixMessageContext fixMessageContext, ref TTarget targetObject);

    /// <summary>
    /// Sets the given value on the referenced target object's property mapped by mapping details.
    /// </summary>
    TTarget SetTarget<TTarget>(ReadOnlySpan<char> value, TagMapLeaf mappingDetails, FixMessageContext fixMessageContext, ref TTarget targetObject);
  }
}
