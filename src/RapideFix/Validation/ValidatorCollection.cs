using System;
using RapideFix.DataTypes;

namespace RapideFix.Validation
{
  public class ValidatorCollection : IValidator
  {
    private readonly FixVersionValidator _versionValidator;
    private readonly ChecksumValidator _checksumValidator;
    private readonly LengthValidator _lengthValidator;

    public ValidatorCollection(IntegerToFixConverter integerConverter)
    {
      _versionValidator = new FixVersionValidator();
      _checksumValidator = new ChecksumValidator(integerConverter);
      _lengthValidator = new LengthValidator(integerConverter);
    }

    public bool PreValidate(ReadOnlySpan<byte> message, FixMessageContext messageContext)
    {
      if(messageContext is null)
      {
        throw new ArgumentNullException(nameof(messageContext));
      }
      if(!_versionValidator.IsValid(message, messageContext))
      {
        return false;
      }
      if(!_lengthValidator.IsValid(message, messageContext))
      {
        return false;
      }
      return true;
    }

    public bool PostValidate(ReadOnlySpan<byte> message, FixMessageContext messageContext)
    {
      if(!_checksumValidator.IsValid(message, messageContext))
      {
        return false;
      }
      return true;
    }
  }
}
