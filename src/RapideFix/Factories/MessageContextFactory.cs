using RapideFix.DataTypes;
using System;

namespace RapideFix.Factories
{
  public class MessageContextFactory
  {
    public FixMessageContext Create(Span<byte> data)
    {
      var result = new FixMessageContext()
      {
        LengthTagStartIndex = data.IndexOf(KnownFixTags.Length),
        MessageTypeTagStartIndex = data.IndexOf(KnownFixTags.MessageType),
        ChecksumTagStartIndex = data.IndexOf(KnownFixTags.Checksum),
      };
      return result;
    }

  }
}
