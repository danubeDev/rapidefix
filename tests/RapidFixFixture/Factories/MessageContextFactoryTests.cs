using RapideFix.DataTypes;
using RapideFix.Factories;
using Xunit;

namespace RapideFixFixture.Factories
{
  public class MessageContextFactoryTests
  {
    [Fact]
    public void GivenSampleMessage_Create_ReturnsMessageContext()
    {
      var message = new TestFixMessageBuilder(TestFixMessageBuilder.DefaultBody).Build();
      var uut = new MessageContextFactory();
      FixMessageContext result = uut.Create(message);

      Assert.NotEqual(-1, result.ChecksumTagStartIndex);
      Assert.NotEqual(-1, result.LengthTagStartIndex);
      Assert.NotEqual(-1, result.MessageTypeTagStartIndex);
    }

  }
}
