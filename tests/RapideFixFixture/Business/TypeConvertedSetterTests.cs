using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using RapideFix.Business;
using RapideFix.Business.Data;
using RapideFix.DataTypes;
using RapideFixFixture.TestTypes;
using Xunit;

namespace RapideFixFixture.Business
{
  public class TypeConvertedSetterTests
  {
    [Fact]
    public void GivenTpyeConverter_SetWithTypeConverter_SetsValueOnTargetObject()
    {
      var targetObject = new TestTypeParent();
      var uut = new TypeConvertedSetter();
      var valueToSet = Encoding.ASCII.GetBytes("12358").AsSpan();
      var property = targetObject.GetType().GetProperty(nameof(targetObject.Tag61));
      var typeConverter = property.GetCustomAttribute<TypeConverterAttribute>();
      var mappingDetails = new TagMapLeaf()
      {
        Current = property,
        TypeConverterName = typeConverter.ConverterTypeName
      };
      var messageContext = new FixMessageContext();
      uut.Set(valueToSet, mappingDetails, messageContext, targetObject);

      Assert.Equal(12358, targetObject.Tag61.Value);
    }

    [Fact]
    public void GivenTpyeConverterOnEnumerable_Set_SetsValueOnTargetObject()
    {
      // Expect the array available for the Tag65
      var targetObject = new TestTypeParent() { Tag65s = new TestConvertable[3] };

      var uut = new TypeConvertedSetter();
      var valueToSet = Encoding.ASCII.GetBytes("12358").AsSpan();
      var property = targetObject.GetType().GetProperty(nameof(targetObject.Tag65s));
      var typeConverter = property.GetCustomAttribute<TypeConverterAttribute>();
      var mappingDetails = new TagMapLeaf()
      {
        IsEnumerable = true,
        Current = property,
        TypeConverterName = typeConverter.ConverterTypeName
      };
      var messageContext = new FixMessageContext();
      uut.Set(valueToSet, mappingDetails, messageContext, targetObject);

      Assert.Equal(12358, targetObject.Tag65s.First().Value);
    }

  }
}
