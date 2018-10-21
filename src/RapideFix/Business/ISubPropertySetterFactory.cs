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
    IPropertySetter GetParentPropertySetter();

    /// <summary>
    /// Returns a property setter for simple and enumerated types.
    /// </summary>
    IPropertySetter GetSimplePropertySetter();

    /// <summary>
    /// Returns a property setter for repeating group tags.
    /// </summary>
    IPropertySetter GetRepeatingGroupTagPropertySetter();

    /// <summary>
    /// Returns a property setter for type converted properties.
    /// </summary>
    IPropertySetter GetTypeConvertedPropertySetter();

    /// <summary>
    /// Returns a property setter for simple and enumerated types for strongly typed target objects.
    /// </summary>
    ITypedPropertySetter GetTypedPropertySetter();
  }
}