namespace RapideFix.Business
{
  public class SubPropertySetterFactory : ISubPropertySetterFactory
  {
    public virtual IPropertySetter GetParentPropertySetter()
    {
      return new ParentTypeSetter();
    }

    public virtual IPropertySetter GetRepeatingGroupTagPropertySetter()
    {
      return new RepeatingGroupTagSetter();
    }

    public virtual IPropertySetter GetSimplePropertySetter()
    {
      return new SimpleTypeSetter();
    }

    public virtual IPropertySetter GetTypeConvertedPropertySetter()
    {
      return new TypeConvertedSetter();
    }
  }
}