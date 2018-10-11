using RapideFix.Extensions;
using System.Runtime.CompilerServices;

namespace RapideFix
{
  internal static class KnownFixTags
  {
    static KnownFixTags()
    {
      MessageType = 35.ToSOHAndKnownTag();
      Checksum = 10.ToSOHAndKnownTag();
      Length = 9.ToSOHAndKnownTag();
      FixVersion = 8.ToKnownTagEquals();
      SenderCompId = 49.ToSOHAndKnownTag();
      TargetCompId = 56.ToSOHAndKnownTag();
    }

    public static byte[] MessageType { get; }

    public static byte[] Checksum { get; }

    public static byte[] Length { get; }

    public static byte[] FixVersion { get; }

    public static byte[] SenderCompId { get; }

    public static byte[] TargetCompId { get; }

    [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
    public static void Initialize()
    {
    }

  }
}
