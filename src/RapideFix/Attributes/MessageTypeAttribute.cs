using System;

namespace RapideFix.Attributes
{
  [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true)]
  public class MessageTypeAttribute : Attribute
  {
    public MessageTypeAttribute(string messageType)
    {
      Value = messageType;
    }

    public string Value { get; }
  }
}
