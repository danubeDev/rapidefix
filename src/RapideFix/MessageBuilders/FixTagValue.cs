using System;
using System.Text;
using RapideFix.DataTypes;

namespace RapideFix.MessageBuilders
{
  public struct FixTagValue : IFixTagValue
  {
    public FixTagValue(int tag, string value, MessageEncoding encoder)
    {
      Tag = tag;
      Encoder = encoder.GetEncoder();
      Value = Encoder.GetBytes(value);
    }

    public FixTagValue(int tag, string value)
    {
      Tag = tag;
      Encoder = Encoding.ASCII;
      Value = Encoder.GetBytes(value);
    }

    public FixTagValue(int tag, int value)
    {
      Tag = tag;
      Encoder = Encoding.ASCII;
      int digitsCount = (int)Math.Floor(Math.Log10(value) + 1);
      var encodedData = new byte[digitsCount];
      IntegerToFixConverter.Instance.Convert(value, into: encodedData, count: digitsCount);
      Value = encodedData;
    }

    public FixTagValue(int tag, double value)
    {
      Tag = tag;
      Encoder = Encoding.ASCII;
      Value = Encoder.GetBytes(value.ToString());
    }

    public int Tag { get; }
    public byte[] Value { get; }
    public Encoding Encoder { get; }

    public int GetLength()
    {
      return Value.Length + Encoding.ASCII.GetByteCount(Tag.ToString()) + 2;
    }

    public int CopyTo(byte[] data, int offset)
    {
      var tagString = $"{Tag}=";
      offset += Encoding.ASCII.GetBytes(tagString, 0, tagString.Length, data, offset);
      Value.CopyTo(data, offset);
      offset += Value.Length;
      data[offset] = Constants.SOHByte;
      return offset + 1;
    }

    public byte[] ToBytes()
    {
      var result = new byte[GetLength()];
      CopyTo(result, 0);
      return result;
    }

    public override string ToString()
    {
      return $"{Tag}={Encoder.GetString(Value)}{Constants.VerticalBar}";
    }
  }
}