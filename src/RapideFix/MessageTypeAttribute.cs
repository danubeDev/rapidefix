﻿using System;

namespace RapideFix
{
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
  public class MessageTypeAttribute : Attribute
  {
    public MessageTypeAttribute(string messageType)
    {
      MessageType = messageType;
    }

    public string MessageType { get; }
  }
}
