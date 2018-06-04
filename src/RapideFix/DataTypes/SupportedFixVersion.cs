using RapideFix.Extensions;
using System;
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

    private SupportedFixVersion(string name)
    {
      _byteValue = name.ToByteValueAndSOH();
      _versionFix = name;
    }

    public static SupportedFixVersion Parse(ReadOnlySpan<byte> fixValue)
    {
      if(TryParse(fixValue, out var result))
      {
        return result;
      }
      throw new NotSupportedException("Given Message Version is not supported.");
    }

    public static bool TryParse(ReadOnlySpan<byte> fixValue, out SupportedFixVersion parsedValue)
    {
      if(fixValue.SequenceEqual(Fix50._byteValue))
      {
        parsedValue = Fix50;
        return true;
      }
      if(fixValue.SequenceEqual(Fix44._byteValue))
      {
        parsedValue = Fix44;
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

    public override string ToString()
    {
      return _versionFix;
    }

  }
}
