using System.Text;
using RapideFix.DataTypes;

namespace RapideFix.MessageBuilders
{
  internal struct FixTagRaw : IFixTagValue
  {
    public FixTagRaw(string data)
    {
      Value = data;
    }

    public string Value { get; }

    public int GetLength()
    {
      return Encoding.ASCII.GetByteCount(Value);
    }

    public int CopyTo(byte[] toArray, int offset)
    {
      var data = Value.Replace(Constants.VerticalBar, Constants.SOHChar);
      offset += Encoding.ASCII.GetBytes(data, 0, data.Length, toArray, offset);
      return offset;
    }

    public byte[] ToBytes()
    {
      var result = new byte[GetLength()];
      CopyTo(result, 0);
      return result;
    }

    public override string ToString()
    {
      return Value;
    }
  }
}
