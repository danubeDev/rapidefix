using System;
using System.Reflection;
using RapideFix.Business.PropertySetters;

namespace RapideFix.Business
{
  /// <summary>
  /// Provides an extensibility point for property setters.
  /// </summary>
  public interface ISubPropertySetterFactory
  {
    /// <summary>
    /// Returns a property setter for parent properties.
    /// </summary>
    IParentSetter GetParentSetter(PropertyInfo property);

    /// <summary>
    /// Returns a property setter for repeating group tags.
    /// </summary>
    BaseSetter GetRepeatingGroupTagSetter(PropertyInfo property);

    /// <summary>
    /// Returns a setter based for a proeperty based on the type or generic type argument of it.
    /// </summary>
    /// <param name="property">The property info of the property to be set.</param>
    /// <param name="typeOfActualProperty">The type or generic type argument of the property.</param>
    /// <returns></returns>
    BaseSetter GetSetter(PropertyInfo property, Type typeOfActualProperty);

    /// <summary>
    /// Returns a property setter for type converted properties.
    /// </summary>
    BaseSetter GetTypeConvertingSetter(PropertyInfo property);
  }
}