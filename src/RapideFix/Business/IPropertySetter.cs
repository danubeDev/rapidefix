using System;
using RapideFix.Business.Data;
using RapideFix.DataTypes;

namespace RapideFix.Business
{
  /// <summary>
  /// Provides methods for types to set parsed byte values on mapped objects.
  /// </summary>
  public interface IPropertySetter
  {
    /// <summary>
    /// Sets the given bytes with the given mapping details on target object
    /// </summary>
    /// <param name="value">The value to be set</param>
    /// <param name="mappingDetails">Mapping details for the property with the tag</param>
    /// <param name="fixMessageContext">Fix message's context data</param>
    /// <param name="targetObject">Target object of the value setter</param>
    /// <returns>The value se or child object created</returns>
    object Set(ReadOnlySpan<byte> value, TagMapLeaf mappingDetails, FixMessageContext fixMessageContext, object targetObject);

    /// <summary>
    /// Sets the given characters with the given mapping details on target object
    /// </summary>
    /// <param name="value">The value to be set</param>
    /// <param name="mappingDetails">Mapping details for the property with the tag</param>
    /// <param name="fixMessageContext">Fix message's context data</param>
    /// <param name="targetObject">Target object of the value setter</param>
    /// <returns>The value se or child object created</returns>
    object Set(ReadOnlySpan<char> value, TagMapLeaf mappingDetails, FixMessageContext fixMessageContext, object targetObject);
  }
}