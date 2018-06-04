using System;
using System.Linq;
using System.Text;
using RapideFix;

namespace RapideFixFixture
{
  internal class TestFixMessageBuilder
  {
    private StringBuilder _tags;
    private string _length;
    private string _beginString;

    public static readonly string DefaultBody = "35=A|49=SERVER|56=CLIENT|34=177|52=20090107-18:15:16|98=0|108=30|";

    public TestFixMessageBuilder()
    {
      _tags = new StringBuilder();
    }

    public TestFixMessageBuilder(string message)
    {
      _tags = new StringBuilder(message);
    }

    public TestFixMessageBuilder AddTag(int tag, string value)
    {
      _tags.Append(tag);
      _tags.Append(Constants.Equal);
      _tags.Append(value);
      _tags.Append(Constants.VerticalBar);
      return this;
    }

    public TestFixMessageBuilder AddTag(string tagAndValue)
    {
      if(!tagAndValue.Contains(Constants.Equal))
      {
        throw new ArgumentException($"Missing '{Constants.Equal}' on tag and value");
      }
      if(tagAndValue.Last() != Constants.VerticalBar)
      {
        throw new ArgumentException("Missing SOH char");
      }
      _tags.Append(tagAndValue);
      return this;
    }

    public TestFixMessageBuilder AddLength(string tagAndValue)
    {
      _length = tagAndValue;
      return this;
    }

    public TestFixMessageBuilder AddBeginString(SupportedFixVersion version)
    {
      _beginString = $"8={version}|";
      return this;
    }

    public TestFixMessageBuilder AddBeginString(string beginString)
    {
      _beginString = beginString;
      return this;
    }

    public byte[] Build()
    {
      StringBuilder sb = new StringBuilder();
      sb.Append(_beginString ?? "8=FIX.4.2|");
      sb.Append(GetLength());
      sb.Append(_tags, 0, _tags.Length);
      var headerAndBody = sb.Replace(Constants.VerticalBar, Constants.SOHChar).ToString();
      return Encoding.ASCII.GetBytes(AddChecksum(headerAndBody));
    }

    public byte[] Build(string checksum)
    {
      StringBuilder sb = new StringBuilder();
      sb.Append(_beginString ?? "8=FIX.4.2|");
      sb.Append(GetLength());
      this.AddTag(checksum);
      sb.Append(_tags, 0, _tags.Length);
      var headerBodyTail = sb.Replace(Constants.VerticalBar, Constants.SOHChar).ToString();
      return Encoding.ASCII.GetBytes(headerBodyTail);
    }

    private string GetLength()
    {
      if(string.IsNullOrEmpty(_length))
      {
        return $"9={_tags.Length}{Constants.VerticalBar}";
      }
      return _length;
    }

    private static string AddChecksum(string headerAndBody)
    {
      var tail = "10=" + CalculateChecksum(headerAndBody) + Constants.SOHChar;
      return headerAndBody + tail;
    }

    private static string CalculateChecksum(string message)
    {
      return (Encoding.ASCII.GetBytes(message).Sum(x => x) % 256).ToString("000");
    }

    public static byte[] CreateDefaultMessage()
    {
      TestFixMessageBuilder builder = new TestFixMessageBuilder(DefaultBody);
      return builder.Build();
    }

  }
}
