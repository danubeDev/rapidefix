using System;
using System.ComponentModel;
using System.Globalization;

namespace RapideFixFixture.TestTypes
{
  public class TestConverter : TypeConverter
  {
    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
    {
      return sourceType == typeof(char[]);
    }

    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
    {
      return new TestConvertable() { Value = int.Parse(((char[])value).AsSpan()) };
    }
  }

}
