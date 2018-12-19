using System.Linq;
using System.Text;
using RapideFix.DataTypes;

namespace RapideFixFixture.MessageBuilders
{
  public static class ConverterTestHelper
  {
    public static byte[] GetEncodedMessage(string data)
    {
      var converted = Encoding.ASCII.GetBytes(data);
      return ReplaceVerticalBar(converted);
    }

    public static byte[] GetEncodedMessage(string begin, byte[] encoded, string end)
    {
      var beginConverted = Encoding.ASCII.GetBytes(begin);
      var endConverted = Encoding.ASCII.GetBytes(end);
      var data = beginConverted.Concat(encoded).Concat(endConverted).ToArray();
      return ReplaceVerticalBar(data);
    }

    private static byte[] ReplaceVerticalBar(byte[] data)
    {
      for(int i = 0; i < data.Length; i++)
      {
        if(data[i] == Constants.VerticalBarByte)
        {
          data[i] = Constants.SOHByte;
        }
      }
      return data;
    }
  }
}
