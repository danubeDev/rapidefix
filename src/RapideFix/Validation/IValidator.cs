using System;
using RapideFix.DataTypes;

namespace RapideFix.Validation
{

  public interface IValidator
  {
    /// <summary>
    /// Returns if the given message is valid.
    /// </summary>
    bool PreValidate(ReadOnlySpan<byte> message, FixMessageContext messageContext);

    /// <summary>
    /// Returns if the given message is valid.
    /// </summary>
    bool PostValidate(ReadOnlySpan<byte> message, FixMessageContext messageContext);
  }
}
