using System;
using System.Reflection;

namespace RapideFix.Business.Data
{
  public abstract class TagMapNode
  {
    public TagMapNode(PropertyInfo current)
    {
      Current = current;
    }

    public PropertyInfo Current { get; }

    public int RepeatingTagNumber { get; internal set; }

    public Type? InnerType { get; internal set; }

    public bool IsEnumerable { get; internal set; }
  }
}
