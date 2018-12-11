using System.Linq;
using System.Text;
using RapideFix;
using RapideFix.DataTypes;
using RapideFix.MessageBuilders;

namespace RapideFixFixture
{
  public class TestFixMessageBuilder : MessageBuilder
  {
    private string _length;
    private string _checksum;
    private string _beginString;

    public static readonly string DefaultBody = "35=A|49=SERVER|56=CLIENT|34=177|52=20090107-18:15:16|98=0|108=30|";

    public TestFixMessageBuilder() : base()
    {
    }

    public TestFixMessageBuilder(string message) : base(message)
    {
    }

    public TestFixMessageBuilder AddLength(string tagAndValue)
    {
      _length = tagAndValue;
      return this;
    }

    public TestFixMessageBuilder AddBeginString(string tagAndValue)
    {
      _beginString = tagAndValue;
      return this;
    }

    public TestFixMessageBuilder AddChecksum(string tagAndValue)
    {
      _checksum = tagAndValue;
      return this;
    }

    public override byte[] Build(out byte checksumValue, out int checksumStart)
    {
      IFixTagValue begin = new FixTagRaw(_beginString ?? $"8={_version ?? SupportedFixVersion.Fix44}|");
      byte[] body = GetBody();
      IFixTagValue length = new FixTagRaw(_length ?? $"9={body.Length}{Constants.VerticalBar}");
      IFixTagValue checksum = GetChecksum(begin.ToBytes(), length.ToBytes(), body, out checksumValue);

      var result = new byte[begin.GetLength() + length.GetLength() + body.Length + checksum.GetLength()];
      int offset = 0;
      offset = begin.CopyTo(result, offset);
      offset = length.CopyTo(result, offset);
      body.CopyTo(result, offset);
      checksumStart = offset + body.Length - 1;
      checksum.CopyTo(result, offset + body.Length);

      return result;
    }

    public override string ToString()
    {
      var begin = new FixTagRaw(_beginString ?? "8=FIX.4.2|");
      var body = GetBody();
      var length = new FixTagRaw(_length ?? $"9={body.Length}{Constants.VerticalBar}");
      IFixTagValue checksum = GetChecksum(begin.ToBytes(), length.ToBytes(), body, out var dummy);

      StringBuilder sb = new StringBuilder();
      sb.Append(begin);
      sb.Append(length);
      foreach(var item in _fixTags)
      {
        sb.Append(item);
      }
      sb.Append(checksum);
      return sb.ToString();
    }

    protected override IFixTagValue GetChecksum(byte[] begin, byte[] length, byte[] body, out byte checksumValue)
    {
      if(_checksum != null)
      {
        checksumValue = 0;
        return new FixTagRaw(_checksum);
      }
      checksumValue = (byte)((begin.Sum(x => x) + length.Sum(x => x) + body.Sum(x => x)) % 256);
      var checksum = checksumValue.ToString("000");
      var tail = "10=" + checksum + Constants.SOHChar;
      return new FixTagRaw(tail);
    }

    public static byte[] CreateDefaultMessage()
    {
      TestFixMessageBuilder builder = new TestFixMessageBuilder(DefaultBody);
      return builder.Build();
    }

  }
}
