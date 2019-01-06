using System;
using RapideFix.DataTypes;

namespace RapideFix.Validation
{
  public class FixVersionValidator : IValidatorInternal
  {
    public bool IsValid(ReadOnlySpan<byte> data, FixMessageContext messageContext)
    {
      if(!data.Slice(0, SupportedFixVersion.TagAndPrefix.Length).SequenceEqual(SupportedFixVersion.TagAndPrefix))
      {
        return false;
      }
      int beginStringLength = data.IndexOf(Constants.SOHByte);
      var fixVersionValue = data.Slice(SupportedFixVersion.TagAndPrefix.Length, beginStringLength - SupportedFixVersion.TagAndPrefix.Length);
      if(!SupportedFixVersion.TryParseEnd(fixVersionValue, out var version))
      {
        return false;
      }
      messageContext.FixVersion = version;
      bool isValid = true;
      if(version == SupportedFixVersion.Fix50)
      {
        var slice = data.Slice(beginStringLength);
        isValid &= slice.IndexOf(KnownFixTags.SenderCompId) >= 0
          && slice.IndexOf(KnownFixTags.TargetCompId) >= 0;
      }
      var msgTypeIndex = messageContext.MessageTypeTagStartIndex;
      var lengthIndex = messageContext.LengthTagStartIndex;
      isValid &= msgTypeIndex > 0 && lengthIndex >= 0;
      return isValid;
    }

  }
}

