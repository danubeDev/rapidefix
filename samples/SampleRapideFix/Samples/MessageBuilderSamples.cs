using System;
using RapideFix;
using RapideFix.MessageBuilders;

namespace SampleRapideFix.Samples
{
  public class MessageBuilderSamples
  {
    public void AddTag()
    {
      byte[] message0 = new MessageBuilder().AddTag(23, "CustomValue").Build();
      //or
      byte[] message1 = new MessageBuilder().AddTag("23=CustomValue|").Build();
    }

    public void AddTagRaw()
    {
      byte[] message = new MessageBuilder().AddRaw("23=CustomValue0|24=CustomValue1|").Build();
    }

    public void AddFixVersion()
    {
      byte[] message = new MessageBuilder().AddBeginString(SupportedFixVersion.Fix42).AddTag(23, "CustomValue").Build();
    }

    public void BuildSpan()
    {
      Span<byte> message = stackalloc byte[40];
      int length = new MessageBuilder().AddTag(23, "CustomValue").Build(message);
      message = message.Slice(0, length);
    }
  }
}
