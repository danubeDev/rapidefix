using System;
using System.Reflection;

namespace RapideFix.Business.Data
{
  public class TagMapParent : TagMapNode
  {
    public TagMapParent(PropertyInfo current, IParentSetter setter) : base(current)
    {
      ParentSetter = setter;
    }

    public IParentSetter ParentSetter { get; }

    public static TagMapParent CreateEnumerable(PropertyInfo property, int repeatingTagNumber, Type innerType, IParentSetter setter)
    {
      return new TagMapParent(property, setter)
      {
        IsEnumerable = true,
        RepeatingTagNumber = repeatingTagNumber,
        InnerType = innerType
      };
    }
  }
}
