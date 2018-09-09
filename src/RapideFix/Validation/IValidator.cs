using RapideFix.DataTypes;
using System;

namespace RapideFix.Validation
{
  public interface IValidator
  {
    /// <summary>
    /// Returns if the given message is valid.
    /// </summary>
    bool IsValid(ReadOnlySpan<byte> message, FixMessageContext messageContext);
  }
}
