using RapideFix.DataTypes;
using System;

namespace RapideFix.Validation
{
  public class FixVersionValidator : IValidator
  {
    public bool IsValid(ReadOnlySpan<byte> data, FixMessageContext messageContext)
    {
      if(!data.StartsWith(KnownFixTags.FixVersion))
      {
        return false;
      }
      int beginStringLength = data.IndexOf(Constants.SOHByte);
      var fixVersionValue = data.Slice(KnownFixTags.FixVersion.Length, beginStringLength - KnownFixTags.FixVersion.Length + 1);
      if(!SupportedFixVersion.TryParse(fixVersionValue, out var version))
      {
        return false;
      }
      messageContext.FixVersion = version;
      var slice = data.Slice(beginStringLength);
      bool isValid = true;
      if(version == SupportedFixVersion.Fix50)
      {
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

