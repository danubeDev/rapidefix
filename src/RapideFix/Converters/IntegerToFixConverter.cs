using System;
using System.Runtime.CompilerServices;
using System.Text;

namespace RapideFix
{
  public class IntegerToFixConverter
  {
    private readonly byte[] _digitToAsciiByte = new byte[10];
    private readonly byte _zero;

    public static IntegerToFixConverter Instance { get; } = new IntegerToFixConverter();

    private IntegerToFixConverter()
    {
      for(int i = 0; i < 10; i++)
      {
        var numberEncoded = Encoding.ASCII.GetBytes(i.ToString());
        if(numberEncoded.Length != 1)
        {
          throw new InvalidOperationException($"Cannot create ASCII byte representation of digits for {i}");
        }
        _digitToAsciiByte[i] = numberEncoded[0];
      }
      _zero = _digitToAsciiByte[0];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Convert(int number, Span<byte> into, int count)
    {
      for(int i = count - 1; i >= 0; i--)
      {
        into[i] = _digitToAsciiByte[number % 10];
        number = number / 10;
      }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int Convert(int number, Span<byte> into)
    {
      int digitsCount = (int)Math.Floor(Math.Log10(number) + 1);
      for(int i = digitsCount - 1; i >= 0; i--)
      {
        into[i] = _digitToAsciiByte[number % 10];
        number = number / 10;
      }
      return digitsCount;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int ConvertBack(ReadOnlySpan<byte> data)
    {
      int digit;
      if(data.Length == 3)
      {
        int digit0 = (data[0] - _zero) * 100;
        int digit1 = (data[1] - _zero) * 10;
        int digit2 = (data[2] - _zero);
        return digit0 + digit1 + digit2;
      }
      if(data.Length == 2)
      {
        int digit0 = (data[0] - _zero) * 10;
        int digit1 = (data[1] - _zero);
        return digit0 + digit1;
      }
      int result = 0;
      foreach(byte b in data)
      {
        digit = b - _zero;
        result = result * 10 + digit;
      }
      return result;
    }
  }
}
