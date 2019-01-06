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
      if(data.Length == 3)
      {
        return (data[0] - _zero) * 100 + (data[1] - _zero) * 10 + (data[2] - _zero);
      }
      if(data.Length == 2)
      {
        return (data[0] - _zero) * 10 + (data[1] - _zero);
      }
      int result = 0;
      for(int i = 0; i < data.Length; i++)
      {
        result = result * 10 + (data[i] - _zero);
      }
      return result;
    }
  }
}
