using System;
using System.Reflection;

namespace RapideFix.Business.Data
{
  public class TagMapNode
  {
    public PropertyInfo Current { get; set; }

    public int RepeatingTagNumber { get; internal set; }

    public Type InnerType { get; internal set; }

    public bool IsEnumerable { get; internal set; }

    public IParentSetter ParentSetter { get; set; }

    public static T CreateEnumerable<T>(PropertyInfo property, int repeatingTagNumber, Type innerType)
      where T : TagMapNode, new()
    {
      return new T()
      {
        Current = property,
        IsEnumerable = true,
        RepeatingTagNumber = repeatingTagNumber,
        InnerType = innerType
      };
    }
  }
}
