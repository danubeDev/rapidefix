﻿using System;
using System.Text;

namespace RapideFix.Extensions
{
  public static class IntegerExtensions
  {
    public static byte[] ToSOHAndKnownTag(this int fixTag)
    {
      var tag = Encoding.ASCII.GetBytes($"{fixTag}=");
      var result = new byte[tag.Length + 1];
      result[0] = Constants.SOHByte;
      Array.Copy(tag, 0, result, 1, tag.Length);
      return result;
    }

    public static byte[] ToKnownTag(this int fixTag)
    {
      var tag = Encoding.ASCII.GetBytes($"{fixTag}=");
      var result = new byte[tag.Length];
      Array.Copy(tag, 0, result, 0, tag.Length);
      return result;
    }
  }
}
