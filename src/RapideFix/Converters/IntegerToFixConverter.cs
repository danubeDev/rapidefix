using System;
using System.Text;

namespace RapideFix
{
  public class IntegerToFixConverter
  {
    private readonly byte[] asciiCachedNumbers = new byte[10];

    public IntegerToFixConverter()
    {
      for(int i = 0; i < 10; i++)
      {
        var numberEncoded = Encoding.ASCII.GetBytes(i.ToString());
        if(numberEncoded.Length != 1)
        {
          throw new InvalidOperationException($"Cannot create ASCII byte representation of digits for {i}");
        }
        asciiCachedNumbers[i] = numberEncoded[0];
      }
    }

    public void Convert(int number, Span<byte> into, int count)
    {
      for(int i = count - 1; i >= 0; i--)
      {
        into[i] = asciiCachedNumbers[number % 10];
        number = number / 10;
      }
    }
  }
}
