using RapideFix.Extensions;
using System;
using System.Runtime.CompilerServices;
using System.Text;

namespace RapideFix
{
  public class SupportedFixVersion
  {
    private readonly byte[] _byteValue;
    private readonly string _versionFix;

    public static readonly SupportedFixVersion Fix42 = new SupportedFixVersion("FIX.4.2");
    public static readonly SupportedFixVersion Fix43 = new SupportedFixVersion("FIX.4.3");
    public static readonly SupportedFixVersion Fix44 = new SupportedFixVersion("FIX.4.4");
    public static readonly SupportedFixVersion Fix50 = new SupportedFixVersion("FIXT.1.1");
    public static readonly byte[] TagAndPrefix = "8=FIX".ToByteValue();
    private static readonly byte[] F42 = ".4.2".ToByteValue();
    private static readonly byte[] F43 = ".4.3".ToByteValue();
    private static readonly byte[] F44 = ".4.4".ToByteValue();
    private static readonly byte[] T11 = "T.1.1".ToByteValue();

    private SupportedFixVersion(string name)
    {
      _byteValue = name.ToByteValue();
      _versionFix = name;
    }

    public ReadOnlySpan<byte> Value => _byteValue;

    public static SupportedFixVersion Parse(ReadOnlySpan<byte> fixValue)
    {
      if(TryParse(fixValue, out var result))
      {
        return result!;
      }
      throw new NotSupportedException("Given Message Version is not supported.");
    }

    public static bool TryParse(ReadOnlySpan<byte> fixValue, out SupportedFixVersion? parsedValue)
    {
      if(fixValue.SequenceEqual(Fix44._byteValue))
      {
        parsedValue = Fix44;
        return true;
      }
      if(fixValue.SequenceEqual(Fix50._byteValue))
      {
        parsedValue = Fix50;
        return true;
      }
      if(fixValue.SequenceEqual(Fix43._byteValue))
      {
        parsedValue = Fix43;
        return true;
      }
      if(fixValue.SequenceEqual(Fix42._byteValue))
      {
        parsedValue = Fix42;
        return true;
      }
      parsedValue = default;
      return false;
    }

    public static bool TryParseEnd(ReadOnlySpan<byte> fixValue, out SupportedFixVersion? parsedValue)
    {
      if(fixValue.SequenceEqual(F44))
      {
        parsedValue = Fix44;
        return true;
      }
      if(fixValue.SequenceEqual(T11))
      {
        parsedValue = Fix50;
        return true;
      }
      if(fixValue.SequenceEqual(F43))
      {
        parsedValue = Fix43;
        return true;
      }
      if(fixValue.SequenceEqual(F42))
      {
        parsedValue = Fix42;
        return true;
      }
      parsedValue = default;
      return false;
    }

    public override string ToString()
    {
      return _versionFix;
    }

  }
}
