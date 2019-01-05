using System;
using System.Reflection;
using RapideFix.Business.PropertySetters;

namespace RapideFix.Business
{
  public class SubPropertySetterFactory : ISubPropertySetterFactory
  {
    public virtual BaseSetter GetSetter(PropertyInfo property, Type typeOfActualProperty)
    {
      if(typeOfActualProperty == typeof(int))
        return new IntegerSetter();
      if(typeOfActualProperty == typeof(double))
        return new DoubleSetter();
      if(typeOfActualProperty == typeof(string))
        return new StringSetter();
      if(typeOfActualProperty == typeof(bool))
        return new BooleanSetter();
      if(typeOfActualProperty == typeof(byte))
        return new ByteSetter();
      if(typeOfActualProperty == typeof(char))
        return new CharSetter();
      if(typeOfActualProperty == typeof(DateTimeOffset))
        return new DateTimeOffsetSetter();
      if(typeOfActualProperty == typeof(decimal))
        return new DecimalSetter();
      if(typeOfActualProperty == typeof(float))
        return new FloatSetter();
      if(typeOfActualProperty == typeof(short))
        return new ShortSetter();
      if(typeOfActualProperty == typeof(long))
        return new LongSetter();

      if(typeOfActualProperty == typeof(int?))
        return new NullableIntegerSetter();
      if(typeOfActualProperty == typeof(double?))
        return new NullableDoubleSetter();
      if(typeOfActualProperty == typeof(bool?))
        return new NullableBooleanSetter();
      if(typeOfActualProperty == typeof(byte?))
        return new NullableByteSetter();
      if(typeOfActualProperty == typeof(char?))
        return new NullableCharSetter();
      if(typeOfActualProperty == typeof(DateTimeOffset?))
        return new NullableDateTimeOffsetSetter();
      if(typeOfActualProperty == typeof(decimal?))
        return new NullableDecimalSetter();
      if(typeOfActualProperty == typeof(float?))
        return new NullableFloatSetter();
      if(typeOfActualProperty == typeof(short?))
        return new NullableShortSetter();
      if(typeOfActualProperty == typeof(long?))
        return new NullableLongSetter();

      throw new NotSupportedException(typeOfActualProperty.Name);
    }

    public virtual BaseSetter GetTypeConvertingSetter(PropertyInfo property)
    {
      return new TypeConvertedSetter();
    }

    public virtual IParentSetter GetParentSetter(PropertyInfo property)
    {
      return new ParentPropertySetter();
    }

    public virtual BaseSetter GetRepeatingGroupTagSetter(PropertyInfo property)
    {
      return new RepeatingGroupTagSetter();
    }
  }
}