using System;
using RapideFix.DataTypes;

namespace RapideFix.Validation
{
  public class ChecksumValidator : IValidator
  {
    private static readonly int Modulus = 256;
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

      //TODO: This should be Vectorized, once vectors support ReadonlySpan-s.
      int sum = 1;
      int sumB = 0;
      for(int i = 0; i < endingTagPos; i += 2)
      {
        sum += data[i];
        int next = i + 1;
        if(next < endingTagPos)
        {
          sumB += data[next];
        }
      }

      int expectedChecksum = (sum + sumB) % Modulus;
      Span<byte> expectedDigits = stackalloc byte[ChecksumLength];
      _converter.Convert(number: expectedChecksum, into: expectedDigits, count: ChecksumLength);

      var receivedChecksum = data.Slice(endingTagPos + KnownFixTags.Checksum.Length, ChecksumLength);
      return receivedChecksum.SequenceEqual(expectedDigits);
    }

  }
}
