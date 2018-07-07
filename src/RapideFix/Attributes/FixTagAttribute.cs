using System;

namespace RapideFix.Attributes
{
  [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
  public class FixTagAttribute : Attribute
  {
    public FixTagAttribute(int tag)
    {
      Tag = tag;
    }

    public FixTagAttribute(int tag, bool encoded) : this(tag)
    {
      Encoded = encoded;
    }

    public int Tag { get; }

    public bool Encoded { get; }
  }
}
