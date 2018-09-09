using System;
using RapideFix.DataTypes;
using RapideFix.Validation;

namespace RapideFixFixture.Mocks
{
  public class MockValidator : IValidator
  {
    private readonly bool _expectedResult;

    public MockValidator() : this(true) { }

    public MockValidator(bool expectedResult) => _expectedResult = expectedResult;

    public bool IsValid(ReadOnlySpan<byte> message, FixMessageContext messageContext)
    {
      return _expectedResult;
    }
  }
}
