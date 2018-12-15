using System;
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
    private byte _checksumValue;
    private int _checksumStart;

    public static readonly string DefaultBody = "35=A|49=SERVER|56=CLIENT|34=177|52=20090107-18:15:16|98=0|108=30|";

    public TestFixMessageBuilder() : base()
    {
    }

    public TestFixMessageBuilder(string message) : base()
    {
      AddRaw(message);
    }

    public TestFixMessageBuilder AddLength(string value)
    {
      _length = value;
      return this;
    }

    public TestFixMessageBuilder AddBeginString(string value)
    {
      _beginString = value;
      return this;
    }

    public TestFixMessageBuilder AddChecksum(string value)
    {
      _checksum = value;
      return this;
    }

    public byte[] Build(out byte checksumValue, out int checksumStart)
    {
      byte[] result = base.Build();
      checksumValue = _checksumValue;
      checksumStart = _checksumStart - 1;
      _beginString = null;
      _checksumValue = 0;
      return result;
    }

    protected override int CalculateRequiredSize()
    {
      int lengthOfLengthValue = (int)Math.Floor(Math.Log10(_currentLength) + 1);
      if(_length != null)
      {
        lengthOfLengthValue = Encoding.ASCII.GetByteCount(_length);
      }
      int lengthOfVersion = _version.Value.Length;
      if(_beginString != null)
      {
        lengthOfVersion = Encoding.ASCII.GetByteCount(_beginString);
      }
      int lengthOfChecksum = 3;
      if(_checksum != null)
      {
        lengthOfChecksum = Encoding.ASCII.GetByteCount(_checksum);
      }

      int expectedLength = _currentLength +
        KnownFixTags.FixVersion.Length + lengthOfVersion + 1 +
        KnownFixTags.Length.Length + lengthOfLengthValue +
        KnownFixTags.Checksum.Length + lengthOfChecksum;

      return expectedLength;
    }

    protected override int AddVersion(Span<byte> into)
    {
      if(_beginString == null)
      {
        return base.AddVersion(into);
      }
      int offset = _converter.Get(8, _beginString, into);
      return offset;
    }

    protected override int AddLength(Span<byte> into, int offset)
    {
      if(_length == null)
      {
        return base.AddLength(into, offset);
      }
      return _converter.Get(9, _length, into);
    }

    protected override int AddChecksum(Span<byte> into, int offset)
    {
      _checksumStart = offset;
      if(_checksum == null)
      {
        _checksumValue = 0;
        for(int i = 0; i < offset; i++)
        {
          _checksumValue += into[i];
        }
        return _converter.Get(10, (int)_checksumValue, into.Slice(offset), 3);
      }

      if(int.TryParse(_checksum, out var parsed))
      {
        _checksumValue = (byte)parsed;
      }
      return _converter.Get(10, _checksum, into);
    }

    protected override void Clear()
    {
      base.Clear();
      _length = null;
      _checksum = null;
    }

    public static byte[] CreateDefaultMessage()
    {
      TestFixMessageBuilder builder = new TestFixMessageBuilder(DefaultBody);
      return builder.Build();
    }

  }
}
