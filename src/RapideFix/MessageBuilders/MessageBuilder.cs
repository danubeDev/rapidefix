using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RapideFix.DataTypes;

namespace RapideFix.MessageBuilders
{
  public class MessageBuilder
  {
    protected List<IFixTagValue> _fixTags;
    protected MessageEncoding _encoding;
    protected SupportedFixVersion _version;

    public MessageBuilder()
    {
      _fixTags = new List<IFixTagValue>();
    }

    public MessageBuilder(string message) : this()
    {
      _fixTags.Add(new FixTagRaw(message));
    }

    public MessageBuilder AddTag(int tag, string value)
    {
      _fixTags.Add(new FixTagValue(tag, value));
      return this;
    }

    public MessageBuilder AddTag(int tag, string value, MessageEncoding encoding)
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

    public MessageBuilder AddTag(string tagAndValue)
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

    public MessageBuilder AddBeginString(SupportedFixVersion version)
    {
      _version = version;
      return this;
    }

    public byte[] Build()
    {
      return Build(out var dummy0, out var dummy1);
    }

    public virtual byte[] Build(out byte checksumValue, out int checksumStart)
    {
      IFixTagValue begin = new FixTagRaw($"8={_version ?? SupportedFixVersion.Fix44}|");
      byte[] body = GetBody();
      IFixTagValue length = new FixTagRaw($"9={body.Length}{Constants.VerticalBar}");
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

    protected byte[] GetBody()
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

    protected virtual IFixTagValue GetChecksum(byte[] begin, byte[] length, byte[] body, out byte checksumValue)
    {
      checksumValue = (byte)((begin.Sum(x => x) + length.Sum(x => x) + body.Sum(x => x)) % 256);
      var checksum = checksumValue.ToString("000");
      var tail = "10=" + checksum + Constants.SOHChar;
      return new FixTagRaw(tail);
    }

  }

}
