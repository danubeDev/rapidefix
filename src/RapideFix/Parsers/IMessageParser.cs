using System;

namespace RapideFix.Parsers
{
  public interface IMessageParser<T>
  {
    /// <summary>
    /// Parses a single message, returns an object of type T, where type mathces  MessageType field
    /// </summary>
    T Parse(ReadOnlySpan<byte> message);

    /// <summary>
    /// Parses a single message, and calls targetObjectFactory function for creation of the targetObject
    /// </summary>
    T Parse(ReadOnlyMemory<byte> message, Func<ReadOnlyMemory<byte>, T> targetObjectFactory);
  }
}