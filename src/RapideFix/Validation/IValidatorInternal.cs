using System;
using RapideFix.DataTypes;

namespace RapideFix.Validation
{
  public interface IValidatorInternal
  {
    /// <summary>
    /// Returns if the given message is valid.
    /// </summary>
    bool IsValid(ReadOnlySpan<byte> message, FixMessageContext messageContext);
  }
}
