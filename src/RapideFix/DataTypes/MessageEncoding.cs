using RapideFix.Extensions;
using System;
using System.Text;
using System.Threading;

namespace RapideFix.DataTypes
{
  public class MessageEncoding
  {
    private readonly byte[] _byteValue;
    private readonly string _encoding;
    private Lazy<Encoding> _encoder;

    public static readonly MessageEncoding JIS = new MessageEncoding("ISO-2022-JP");
    public static readonly MessageEncoding EUC = new MessageEncoding("EUC-JP");
    public static readonly MessageEncoding UTF8 = new MessageEncoding("UTF-8");
    //Note, that Shift-JIS is not supported by .net

    private MessageEncoding(string encoding)
    {
      _byteValue = encoding.ToByteValueAndSOH();
      _encoding = encoding.ToLowerInvariant();
      _encoder = new Lazy<Encoding>(LoadEncoder, LazyThreadSafetyMode.None);
    }

    public static MessageEncoding Parse(ReadOnlySpan<byte> fixValue)
    {
      if(TryParse(fixValue, out var result))
      {
        return result!;
      }
      throw new NotSupportedException("Given Message Encoding is not supported.");
    }

    public static bool TryParse(ReadOnlySpan<byte> fixValue, out MessageEncoding? parsedValue)
    {
      if(fixValue.SequenceEqual(UTF8._byteValue))
      {
        parsedValue = UTF8;
        return true;
      }
      if(fixValue.SequenceEqual(JIS._byteValue))
      {
        parsedValue = JIS;
        return true;
      }
      if(fixValue.SequenceEqual(EUC._byteValue))
      {
        parsedValue = EUC;
        return true;
      }
      parsedValue = default;
      return false;
    }

    public override string ToString()
    {
      return _encoding;
    }

    public byte[] FixEncodedValue =>  _byteValue;

    public Encoding GetEncoder() => _encoder.Value;

    private Encoding LoadEncoder()
    {
      return Encoding.GetEncoding(_encoding);
    }
  }
}
