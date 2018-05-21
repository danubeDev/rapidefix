using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Buffers.Text;
using RapideFix.StringProcessing;
using RapideFix.Attributes;

namespace RapideFix
{
  public class FixParser
  {
    private readonly FixParserSettings _settings;
    
    public FixParser(FixParserSettings settings)
    {
      _settings = settings;
    }

    public TMessage Parse<TMessage>(string data) where TMessage : new()
    {
      TMessage message = new TMessage();
      Dictionary<int, PropertyInfo> tagFixTagAttributeMap = GetPropertyFixTagAttributes(typeof(TMessage));

      foreach (ReadOnlyMemory<char> item in new FixStringEnumerable(data))
      {
        (ReadOnlyMemory<char> fixTagChars, ReadOnlyMemory<char> value) = item.GetTagAndValue();
        int fixTag = int.Parse(fixTagChars.Span);

        if (tagFixTagAttributeMap.TryGetValue(fixTag, out var propertyInfo))
        {
          propertyInfo.SetValue(message, Parse(propertyInfo.PropertyType, value));
        }
      }

      return message;
    }

    public TMessage Parse<TMessage>(byte[] data)
    {
      throw new NotImplementedException();
    }

    public TMessage Parse<TMessage>(Stream data)
    {
      throw new NotImplementedException();
    }

    private ReadOnlySpan<char> GetNextElement(int start, string str)
    {
      return str.AsSpan(start, str.IndexOf(Constant.VerticalBar, start)-start);
    }

    #region Private Methods

    private ReadOnlySpan<char> GetValue(ReadOnlySpan<char> element)
    {
      int index = element.IndexOf(Constant.Equal) + 1;
      return element.Slice(index, element.Length - index);
    }

    private Dictionary<int, PropertyInfo> GetPropertyFixTagAttributes(Type type)
    {
      Dictionary<int, PropertyInfo> fixTagPropertyInfoMap = new Dictionary<int, PropertyInfo>();
      PropertyInfo[] properties = type.GetProperties();

      foreach (PropertyInfo propertyInfo in properties)
      {
        FixTagAttribute fixTagAttribute = (FixTagAttribute)propertyInfo.GetCustomAttribute(typeof(FixTagAttribute), false);
        fixTagPropertyInfoMap.Add(fixTagAttribute.Tag, propertyInfo);
      }

      return fixTagPropertyInfoMap;
    }

    private object Parse(Type type, ReadOnlyMemory<char> value)
    {
      switch (type.Name)
      {
        case "Int32":
          return int.Parse(value.Span);
        case "String":
          return value.Span.ToString();
        case "Double":
          return double.Parse(value.ToString()); //Span has issues with double parsing right now
      }
      return null;
    }

    #endregion
  }
}
