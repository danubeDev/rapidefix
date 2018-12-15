using System;
using System.Text;
using RapideFix.DataTypes;

namespace RapideFix.MessageBuilders
{
  public class FixTagValueConverter
  {
    public int Get(int tag, string value, MessageEncoding encoder, Span<byte> into)
    {
      var encode = encoder.GetEncoder();
      var currentOffset = IntegerToFixConverter.Instance.Convert(tag, into);
      int result = currentOffset;
      into = into.Slice(currentOffset);
      into[0] = Constants.EqualsByte;
      into = into.Slice(1);
      currentOffset = encode.GetBytes(value, into);
      result += currentOffset;
      into = into.Slice(currentOffset);
      into[0] = Constants.SOHByte;
      return result + 2;
    }

    public int Get(int tag, string value, Span<byte> into)
    {
      var currentOffset = IntegerToFixConverter.Instance.Convert(tag, into);
      int result = currentOffset;
      into = into.Slice(currentOffset);
      into[0] = Constants.EqualsByte;
      into = into.Slice(1);
      currentOffset = Encoding.ASCII.GetBytes(value, into);
      result += currentOffset;
      into = into.Slice(currentOffset);
      into[0] = Constants.SOHByte;
      return result + 2;
    }

    public int Get(string value, Span<byte> into)
    {
      var offset = Encoding.ASCII.GetBytes(value, into);
      for(int i = 0; i < offset; i++)
      {
        if(into[i] == Constants.VerticalBarByte)
        {
          into[i] = Constants.SOHByte;
        }
      }
      return offset;
    }

    public int Get(int tag, int value, Span<byte> into)
    {
      var currentOffset = IntegerToFixConverter.Instance.Convert(tag, into);
      int result = currentOffset;
      into = into.Slice(currentOffset);
      into[0] = Constants.EqualsByte;
      into = into.Slice(1);
      currentOffset = IntegerToFixConverter.Instance.Convert(value, into);
      result += currentOffset;
      into = into.Slice(currentOffset);
      into[0] = Constants.SOHByte;
      return result + 2;
    }

    public int Get(int tag, int value, Span<byte> into, int fixedLength)
    {
      var currentOffset = IntegerToFixConverter.Instance.Convert(tag, into);
      int result = currentOffset;
      into = into.Slice(currentOffset);
      into[0] = Constants.EqualsByte;
      into = into.Slice(1);
      IntegerToFixConverter.Instance.Convert(value, into, fixedLength);
      result += fixedLength;
      into = into.Slice(fixedLength);
      into[0] = Constants.SOHByte;
      return result + 2;
    }

    public int Get(int tag, double value, Span<byte> into)
    {
      return Get(tag, value.ToString(), into);
    }
  }
}