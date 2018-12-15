using System;
using RapideFix.DataTypes;

namespace RapideFix.Validation
{
  public class ChecksumValidator : IValidatorInternal
  {
    private readonly IntegerToFixConverter _converter;
    private static readonly int ChecksumLength = 3;

    public ChecksumValidator(IntegerToFixConverter converter)
    {
      _converter = converter ?? throw new ArgumentNullException(nameof(converter));
    }

    public bool IsValid(ReadOnlySpan<byte> data, FixMessageContext msgContext)
    {
      var endingTagPos = msgContext.ChecksumTagStartIndex;
      if(endingTagPos < 0 || (endingTagPos + KnownFixTags.Checksum.Length + ChecksumLength + 1) != data.Length)
      {
        return false;
      }

      Span<byte> expectedDigits = stackalloc byte[ChecksumLength];
      _converter.Convert(number: msgContext.ChecksumValue, into: expectedDigits, count: ChecksumLength);

      var receivedChecksum = data.Slice(endingTagPos + KnownFixTags.Checksum.Length, ChecksumLength);
      return receivedChecksum.SequenceEqual(expectedDigits);
    }

  }
}
