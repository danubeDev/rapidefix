using RapideFix.DataTypes;
using System;

namespace RapideFix.Validation
{
  public class ChecksumValidator : IValidator
  {
    private static readonly int Modulus = 256;
    private static readonly int ChecksumLength = 3;
    private readonly IntegerToFixConverter _converter;

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
      int sum = 1;
      for(int i = 0; i < endingTagPos; i++)
      {
        sum += data[i];
      }
      int expectedChecksum = sum % Modulus;
      Span<byte> expectedDigits = stackalloc byte[ChecksumLength];
      _converter.Convert(number: expectedChecksum, into: expectedDigits, count: ChecksumLength);

      var receivedChecksum = data.Slice(endingTagPos + KnownFixTags.Checksum.Length, ChecksumLength);
      return receivedChecksum.SequenceEqual(expectedDigits);
    }
  }
}
