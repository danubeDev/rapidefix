using System;
using System.Text;

namespace RapideFix.Extensions
{
  public static class StringExtensions
  {
    public static byte[] ToByteValueAndSOH(this string fixValue)
    {
      var value = Encoding.ASCII.GetBytes(fixValue);
      var result = new byte[value.Length + 1];
      Array.Copy(value, 0, result, 0, value.Length);
      result[value.Length] = Constants.SOHByte;
      return result;
    }

    public static byte[] ToByteValue(this string fixValue)
    {
      return Encoding.ASCII.GetBytes(fixValue);
    }

  }
}
