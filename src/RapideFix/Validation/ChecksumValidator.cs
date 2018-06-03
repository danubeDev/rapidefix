using System;
using System.Runtime.CompilerServices;
using System.Text;

namespace RapideFix.Validation
{
  public class ChecksumValidator
  {
    private static readonly int Modulus = 256;
    private static readonly int ChecksumLength = 3;
    private readonly IntegerToFixConverter _converter;

    public ChecksumValidator(IntegerToFixConverter converter)
    {
      _converter = converter ?? throw new ArgumentNullException(nameof(converter));
    }

    public bool IsValid(string data)
    {
      throw new NotImplementedException();
    }

    public bool IsValid(Span<byte> data)
    {
      int endingTagPos = data.LastIndexOf(KnownFixTags.Checksum);
      return IsValid(data, endingTagPos);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsValid(Span<byte> data, int endingTagPos)
    {
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
