using System;
using System.Collections.Generic;
using System.Reflection;

namespace RapideFix.Business.Data
{
  public class TagMapLeaf : TagMapNode
  {
    public List<TagMapNode> Parents { get; set; }

    public string TypeConverterName { get; set; }

    public ITypedPropertySetter Setter { get; set; }

    public bool IsEncoded { get; set; }

    public bool IsRepeatingGroupTag { get; set; }

    public static T CreateRepeatingTag<T>(PropertyInfo property, Type innerType)
      where T : TagMapLeaf, new()
    {
      return new T()
      {
        Current = property,
        IsEnumerable = false,
        IsRepeatingGroupTag = true,
        InnerType = innerType
      };
    }
  }
}
