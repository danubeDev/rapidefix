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

  }
}
