using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RapideFix;
using RapideFix.DataTypes;

namespace RapideFixFixture
{
  public class TestFixMessageBuilder
  {
    private List<IFixTagValue> _fixTags;
    private MessageEncoding _encoding;
    private string _length;
    private string _beginString;
    private string _checksum;

    public static readonly string DefaultBody = "35=A|49=SERVER|56=CLIENT|34=177|52=20090107-18:15:16|98=0|108=30|";

    public TestFixMessageBuilder()
    {
      _fixTags = new List<IFixTagValue>();
    }

    public TestFixMessageBuilder(string message) : this()
    {
      _fixTags.Add(new FixTagRaw(message));
    }

    public TestFixMessageBuilder AddTag(int tag, string value)
    {
      _fixTags.Add(new FixTagValue(tag, value));
      return this;
    }

    public TestFixMessageBuilder AddTag(int tag, string value, MessageEncoding encoding)
    {
      if(_encoding == null)
      {
        _encoding = encoding;
      }
      else if(_encoding != encoding)
      {
        throw new InvalidOperationException("Encoding is already set");
      }
      _fixTags.Add(new FixTagValue(tag, value, encoding));
      return this;
    }

    public TestFixMessageBuilder AddTag(string tagAndValue)
    {
      if(!tagAndValue.Contains(Constants.Equal))
      {
        throw new ArgumentException($"Missing '{Constants.Equal}' on tag and value");
      }
      if(tagAndValue.Last() != Constants.VerticalBar)
      {
        throw new ArgumentException("Missing SOH char");
      }
      _fixTags.Add(new FixTagRaw(tagAndValue));
      return this;
    }

    public TestFixMessageBuilder AddLength(string tagAndValue)
    {
      _length = tagAndValue;
      return this;
    }

    public TestFixMessageBuilder AddBeginString(SupportedFixVersion version)
    {
      _beginString = $"8={version}|";
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

    public byte[] Build()
    {
      return Build(out var dummy);
    }

    public byte[] Build(out int checksumValue)
    {
      IFixTagValue begin = new FixTagRaw(_beginString ?? "8=FIX.4.4|");
      byte[] body = GetBody();
      IFixTagValue length = new FixTagRaw(_length ?? $"9={body.Length}{Constants.VerticalBar}");
      IFixTagValue checksum = GetChecksum(begin.ToBytes(), length.ToBytes(), body, out checksumValue);

      var result = new byte[begin.GetLength() + length.GetLength() + body.Length + checksum.GetLength()];
      int offset = 0;
      offset = begin.CopyTo(result, offset);
      offset = length.CopyTo(result, offset);
      body.CopyTo(result, offset);
      checksum.CopyTo(result, offset + body.Length);

      return result;
    }

    private byte[] GetBody()
    {
      var bodyLength = _fixTags.Sum(x => x.GetLength());
      byte[] result = new byte[bodyLength];
      int offset = 0;
      foreach(var encoded in _fixTags)
      {
        offset = encoded.CopyTo(result, offset);
      }
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

    private IFixTagValue GetChecksum(byte[] begin, byte[] length, byte[] body, out int checksumValue)
    {
      if(_checksum != null)
      {
        checksumValue = -1;
        return new FixTagRaw(_checksum);
      }
      checksumValue = (begin.Sum(x => x) + length.Sum(x => x) + body.Sum(x => x)) % 256;
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
