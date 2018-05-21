using System;

namespace RapideFix.StringProcessing
{
  public static class ReadOnlyMemoryExtensions
  {
    public static ReadOnlyMemory<char> GetTag<T>(this ReadOnlyMemory<char> fixElement)
    {
      int tagValueSeparatorIndex = fixElement.GetTagValueSeparatorIndex();
      return fixElement.Slice(0, tagValueSeparatorIndex);
    }

    public static ReadOnlyMemory<char> GetValue<T>(this ReadOnlyMemory<char> fixElement)
    {
      int tagValueSeparatorIndex = fixElement.GetTagValueSeparatorIndex();
      return fixElement.Slice(tagValueSeparatorIndex + 1);
    }

    public static (ReadOnlyMemory<char> tag, ReadOnlyMemory<char> value) GetTagAndValue(this ReadOnlyMemory<char> fixElement)
    {
      int tagValueSeparatorIndex = fixElement.GetTagValueSeparatorIndex();
      return (fixElement.Slice(0, tagValueSeparatorIndex), fixElement.Slice(tagValueSeparatorIndex + 1));
    }

    private static int GetTagValueSeparatorIndex(this ReadOnlyMemory<char> fixElement)
    {
      return fixElement.Span.IndexOf(Constant.Equal);
    }
  }
}
