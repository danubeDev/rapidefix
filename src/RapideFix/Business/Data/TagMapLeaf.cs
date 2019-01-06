using System;
using System.Collections.Generic;
using System.Reflection;
using RapideFix.Business.PropertySetters;

namespace RapideFix.Business.Data
{
  public class TagMapLeaf : TagMapNode
  {
    public List<TagMapNode> Parents { get; set; }

    public string TypeConverterName { get; set; }

    public bool IsEncoded { get; set; }

    public BaseSetter Setter { get; set; }

    public static T CreateRepeatingTag<T>(PropertyInfo property, Type innerType, BaseSetter propertySetter)
      where T : TagMapLeaf, new()
    {
      return new T()
      {
        Current = property,
        IsEnumerable = false,
        InnerType = innerType,
        Setter = propertySetter
      };
    }
  }
}
