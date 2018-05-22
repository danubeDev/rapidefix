using System;

namespace RapideFix.Attributes
{
  [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
  public class FixTagAttribute : Attribute
  {
    public FixTagAttribute(int tag)
    {
      Tag = tag;
    }

    public int Tag { get; }
  }
}
