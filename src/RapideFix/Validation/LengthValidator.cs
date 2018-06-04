using RapideFix.DataTypes;
using System;

namespace RapideFix.Validation
{
  public class LengthValidator : IValidator
  {
    private readonly IntegerToFixConverter _converter;
    public LengthValidator(IntegerToFixConverter converter)
    {
      _converter = converter ?? throw new ArgumentNullException(nameof(converter));
    }

    public bool IsValid(Span<byte> data, FixMessageContext msgContext)
    {
      var endingTagPos = msgContext.ChecksumTagStartIndex;
      int startingTagPos = msgContext.MessageTypeTagStartIndex;
      int expectedLength = endingTagPos - startingTagPos;
      if(expectedLength <= 0)
      {
        return false;
      }
      int digitsCount = (int)Math.Floor(Math.Log10(expectedLength) + 1);
      Span<byte> expectedDigits = stackalloc byte[digitsCount + 1];
      _converter.Convert(number: expectedLength, into: expectedDigits, count: digitsCount);
      expectedDigits[digitsCount] = Constants.SOHByte;

      int lengthTagPos = msgContext.LengthTagStartIndex;
      var fromLengthStart = data.Slice(lengthTagPos + KnownFixTags.Length.Length);

      return fromLengthStart.StartsWith(expectedDigits);
    }

  }
}