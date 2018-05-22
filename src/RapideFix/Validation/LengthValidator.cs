using RapideFix;
using System;
using System.Runtime.CompilerServices;

namespace RapideFixFixture.Validation
{
  public class LengthValidator
  {
    private readonly IntegerToFixConverter _converter;
    public LengthValidator(IntegerToFixConverter converter)
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
      int startingTagPos = data.IndexOf(KnownFixTags.MessageType);
      int expectedLength = endingTagPos - startingTagPos;
      if(expectedLength <= 0)
      {
        return false;
      }
      int digitsCount = (int)Math.Floor(Math.Log10(expectedLength) + 1);
      Span<byte> expectedDigits = stackalloc byte[digitsCount + 1];
      _converter.Convert(number: expectedLength, into: expectedDigits, count: digitsCount);
      expectedDigits[digitsCount] = Constants.SOHByte;

      int lengthTagPos = data.IndexOf(KnownFixTags.Length);
      var fromLengthStart = data.Slice(lengthTagPos + KnownFixTags.Length.Length);

      return fromLengthStart.StartsWith(expectedDigits);
    }

  }
}