using System;
using RapideFix.Parsers;
using RapideFixFixture.TestTypes;

namespace RapideFixFixture.Mocks
{
  public class MockMessageParser : IMessageParser<TestTypeParent>
  {
    public TestTypeParent Parse(ReadOnlySpan<byte> message)
    {
      return new TestTypeParent();
    }

    public TestTypeParent Parse(ReadOnlyMemory<byte> message, Func<ReadOnlyMemory<byte>, TestTypeParent> targetObjectFactory)
    {
      return new TestTypeParent();
    }
  }
}
