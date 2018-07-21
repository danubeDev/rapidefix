using System.Runtime.CompilerServices;

[assembly:InternalsVisibleTo("RapideFixFixture")]
namespace RapideFix
{
  internal static class Constants
  {
    internal const char SOHChar = '\u0001';
    internal const char VerticalBar = '|';
    internal const char Equal = '=';

    internal const byte SOHByte = 1;

    internal const char True = 'Y';
    internal const char TrueNumber = '1';

    internal const char False = 'N';
    internal const char FalseNumber = '0';

  }
}
