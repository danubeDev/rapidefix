using RapideFix;
using System;
using System.Text;

namespace RapideFixFixture.Validation
{
  public class LengthValidator
  {
    private readonly byte[] asciiCachedNumbers = new byte[10];

    public LengthValidator()
    {
      for(int i = 0; i < 10; i++)
      {
        var numberEncoded = Encoding.ASCII.GetBytes(i.ToString());
        if(numberEncoded.Length > 1)
        {
          throw new InvalidOperationException($"Cannot create ASCII byte representation of digits for {i}");
        }
        asciiCachedNumbers[i] = numberEncoded[0];
      }
    }

    public bool IsValid(string data)
    {
      throw new NotImplementedException();
    }

    public bool IsValid(Span<byte> data)
    {
      int startingTagPos = data.IndexOf(KnownValues.MessageType);
      int endingTagPos = data.IndexOf(KnownValues.Checksum);
      int expectedLength = endingTagPos - startingTagPos;
      if(expectedLength <= 0)
      {
        return false;
      }
      int digitsCount = (int)Math.Floor(Math.Log10(expectedLength) + 1);
      Span<byte> expectedDigits = stackalloc byte[digitsCount + 1];
      SetDigits(number: expectedLength, into: expectedDigits, count: digitsCount);
      expectedDigits[digitsCount] = Constants.SOHByte;

      int lengthTagPos = data.IndexOf(KnownValues.Length);
      var fromLengthStart = data.Slice(lengthTagPos + KnownValues.Length.Length);

      return fromLengthStart.StartsWith(expectedDigits);
    }

    private void SetDigits(int number, Span<byte> into, int count)
    {
      for(int i = count - 1; i >= 0; i--)
      {
        into[i] = asciiCachedNumbers[number % 10];
        number = number / 10;
      }
    }
  }
}