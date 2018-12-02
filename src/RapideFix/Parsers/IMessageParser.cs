using System;

namespace RapideFix.Parsers
{
  public interface IMessageParser<TOutput, TData>
  {
    /// <summary>
    /// Parses a single message, returns an object of type T, where type mathces  MessageType field
    /// </summary>
    TOutput Parse(ReadOnlySpan<TData> message);

    /// <summary>
    /// Parses a single message, and calls targetObjectFactory function for creation of the targetObject
    /// </summary>
    TOutput Parse(ReadOnlyMemory<TData> message, Func<ReadOnlyMemory<TData>, TOutput> targetObjectFactory);
  }
}