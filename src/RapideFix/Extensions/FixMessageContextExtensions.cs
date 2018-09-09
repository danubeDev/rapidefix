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
      messageContext.LengthTagStartIndex = data.IndexOf(KnownFixTags.Length);
      messageContext.MessageTypeTagStartIndex = data.IndexOf(KnownFixTags.MessageType);
      messageContext.ChecksumTagStartIndex = data.IndexOf(KnownFixTags.Checksum);
      messageContext.EncodedFields = encoding;
      return messageContext;
    }

    public static FixMessageContext CleanUp(this FixMessageContext data)
    {
      data.ChecksumTagStartIndex = -1;
      data.LengthTagStartIndex = -1;
      data.MessageTypeTagStartIndex = -1;
      data.EncodedFields = null;
      data.FixVersion = null;
      data.RepeatingGroupCounters?.Clear();
      data.CreatedParentTypes?.Clear();
      return data;
    }
  }
}
