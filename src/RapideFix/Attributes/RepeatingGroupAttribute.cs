using System;

namespace RapideFix.Attributes
{
  [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
  public class RepeatingGroupAttribute : Attribute
  {
    public RepeatingGroupAttribute(int repeatingGroupTag) => Tag = repeatingGroupTag;

    public int Tag { get; }

  }
}
