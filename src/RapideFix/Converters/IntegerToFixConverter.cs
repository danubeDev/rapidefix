using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace RapideFix
{
  public class IntegerToFixConverter
  {
    private readonly byte[] digitToAsciiByte = new byte[10];
    private readonly Dictionary<byte, int> asciiByteToInt = new Dictionary<byte, int>();

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
        digitToAsciiByte[i] = numberEncoded[0];
        asciiByteToInt.Add(numberEncoded[0], i);
      }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Convert(int number, Span<byte> into, int count)
    {
      for(int i = count - 1; i >= 0; i--)
      {
        into[i] = digitToAsciiByte[number % 10];
        number = number / 10;
      }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int ConvertBack(ReadOnlySpan<byte> data)
    {
      int result = 0;
      foreach(byte b in data)
      {
        if(asciiByteToInt.TryGetValue(b, out int digit))
        {
          result = result * 10 + digit;
        }
      }
      return result;
    }
  }
}
