using System;
using System.Text;

namespace RapideFix.Validation
{
  public class ChecksumValidator
  {
    private static readonly int Modulus = 256;
    private static readonly int ChecksumLength = 3;

    public ChecksumValidator()
    {
      KnownValues.Initialize();
    }

    public bool IsValid(string data)
    {
      throw new NotImplementedException();
    }

    public bool IsValid(Span<byte> data)
    {
      int endingTagPos = data.IndexOf(KnownValues.Checksum);
      if(endingTagPos < 0 || (endingTagPos + KnownValues.Checksum.Length + ChecksumLength + 1) != data.Length)
      {
        return false;
      }
      int sum = 1;
      for(int i = 0; i < endingTagPos; i++)
      {
        sum += data[i];
      }
      int expectedChecksum = sum % Modulus;
      var slice = data.Slice(endingTagPos + KnownValues.Checksum.Length, ChecksumLength);
      Span<char> charsOfChecksum = stackalloc char[ChecksumLength];
      Encoding.ASCII.GetChars(slice, charsOfChecksum);
      return int.TryParse(charsOfChecksum, out var receivedChecksum) && expectedChecksum == receivedChecksum;
    }
  }
}
