using System;
using System.Runtime.CompilerServices;
using System.Text;

namespace RapideFix
{
  internal static class KnownFixTags
  {
    static KnownFixTags()
    {
      MessageType = CreateKnownTag(35);
      Checksum = CreateKnownTag(10);
      Length = CreateKnownTag(9);
    }

    public static byte[] MessageType { get; }

    public static byte[] Checksum { get; }

    public static byte[] Length { get; }

    [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
    public static void Initialize()
    {
    }

    private static byte[] CreateKnownTag(int fixTag)
    {
      var tag = Encoding.ASCII.GetBytes($"{fixTag}=");
      var result = new byte[tag.Length + 1];
      result[0] = Constants.SOHByte;
      Array.Copy(tag, 0, result, 1, tag.Length);
      return result;
    }

  }
}
