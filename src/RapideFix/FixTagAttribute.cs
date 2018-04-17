using System;

namespace RapideFix
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
