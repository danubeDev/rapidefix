using System;
using System.Collections.Generic;
using RapideFix.DataTypes;

namespace RapideFix.Validation
{
  public class ValidatorCollection : IValidator
  {
    private readonly List<IValidator> _validators;

    public ValidatorCollection(IntegerToFixConverter integerConverter)
    {
      _validators = new List<IValidator>();
      _validators.Add(new FixVersionValidator());
      _validators.Add(new ChecksumValidator(integerConverter));
      _validators.Add(new LengthValidator(integerConverter));
    }

    public bool IsValid(Span<byte> message, FixMessageContext messageContext)
    {
      if(messageContext == null)
      {
        throw new ArgumentNullException(nameof(messageContext));
      }
      foreach(var validator in _validators)
      {
        if(!validator.IsValid(message, messageContext))
        {
          return false;
        }
      }
      return true;
    }
  }
}
