using System;
using System.Collections.Generic;
using System.Reflection;
using RapideFix.Business.PropertySetters;

namespace RapideFix.Business.Data
{
  public class TagMapLeaf : TagMapNode
  {
    public TagMapLeaf(PropertyInfo current, BaseSetter setter) : base(current)
    {
      Setter = setter;
    }

    public List<TagMapParent>? Parents { get; set; }

    public string? TypeConverterName { get; set; }

    public bool IsEncoded { get; set; }

    public BaseSetter Setter { get; }

    public static TagMapLeaf CreateRepeatingTag(PropertyInfo property, Type innerType, BaseSetter propertySetter)
    {
      return new TagMapLeaf(property, propertySetter)
      {
        IsEnumerable = false,
        InnerType = innerType,
      };
    }

    public static TagMapLeaf CreateEnumerable(PropertyInfo property, int repeatingTagNumber, Type innerType, BaseSetter setter)
    {
      return new TagMapLeaf(property, setter)
      {
        IsEnumerable = true,
        RepeatingTagNumber = repeatingTagNumber,
        InnerType = innerType
      };
    }
  }
}
