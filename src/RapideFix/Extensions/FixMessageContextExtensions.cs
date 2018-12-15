using System;
using RapideFix.DataTypes;

namespace RapideFix.Extensions
{
  public static class FixMessageContextExtensions
  {
    public static FixMessageContext Setup(this FixMessageContext messageContext, ReadOnlySpan<byte> data)
    {
      return Setup(messageContext, data, null);
    }

    public static FixMessageContext Setup(this FixMessageContext messageContext, ReadOnlySpan<byte> data, MessageEncoding encoding)
    {
      messageContext.FixVersion = null;
      messageContext.EncodedFields = encoding;
      messageContext.RepeatingGroupCounters?.Clear();
      messageContext.CreatedParentTypes?.Clear();
      messageContext.ChecksumValue = 0;
      messageContext.LengthTagStartIndex = data.IndexOf(KnownFixTags.Length);
      messageContext.MessageTypeTagStartIndex = data.IndexOf(KnownFixTags.MessageType);
      messageContext.ChecksumTagStartIndex = data.IndexOf(KnownFixTags.Checksum);
      return messageContext;
    }

    public static FixMessageContext Setup(this FixMessageContext messageContext, ReadOnlySpan<char> data)
    {
      messageContext.RepeatingGroupCounters?.Clear();
      messageContext.CreatedParentTypes?.Clear();
      return messageContext;
    }
  }
}
