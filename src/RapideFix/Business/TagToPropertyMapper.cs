using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using RapideFix.Attributes;
using RapideFix.Business.Data;

namespace RapideFix.Business
{
  public class TagToPropertyMapper : ITagToPropertyMapper
  {
    private Dictionary<int, TagMapLeaf> _map = new Dictionary<int, TagMapLeaf>();
    private Dictionary<int, Type> _mapMessageType = new Dictionary<int, Type>();
    private HashSet<Type> _mappedTypes = new HashSet<Type>();
    private static NumberFormatInfo _numberFormatInfo = CultureInfo.CurrentCulture.NumberFormat;

    public bool TryGet(ReadOnlySpan<byte> tag, out TagMapLeaf result)
    {
      int key = IntegerToFixConverter.Instance.ConvertBack(tag);
      return _map.TryGetValue(key, out result);
    }

    public bool TryGet(ReadOnlySpan<char> tag, out TagMapLeaf result)
    {
      if(!int.TryParse(tag, NumberStyles.Integer, _numberFormatInfo, out int key))
      {
        result = null;
        return false;
      }
      return _map.TryGetValue(key, out result);
    }

    public Type TryGetMessageType(ReadOnlySpan<byte> tag)
    {
      _mapMessageType.TryGetValue(GetTypeKey(tag), out var result);
      return result;
    }

    public void Map<T>()
    {
      var type = typeof(T);
      Map(type);
    }

    public void Map(Type type)
    {
      Map(type, new Stack<TagMapNode>());
    }

    private void Map(Type type, Stack<TagMapNode> parents)
    {
      if(_mappedTypes.Contains(type) || type == typeof(object))
      {
        return;
      }
      _mappedTypes.Add(type);
      var messageType = type.GetCustomAttribute<MessageTypeAttribute>();
      if(messageType != null)
      {
        var key = GetTypeKey(Encoding.ASCII.GetBytes(messageType.Value));
        _mapMessageType.TryAdd(key, type);
      }

      foreach(PropertyInfo property in type.GetProperties())
      {
        FixTagAttribute fixTagAttribute = property.GetCustomAttribute<FixTagAttribute>();
        var repeatingGroup = property.GetCustomAttribute<RepeatingGroupAttribute>();
        var isEnumerable = property.PropertyType.GetInterfaces()
               .Any(x => x == typeof(System.Collections.IEnumerable))
               && property.PropertyType != typeof(string);
        Type innerType = null;
        if(repeatingGroup != null)
        {
          AddRepeatingGroupLeaf(parents, property, repeatingGroup, GetInnerTypeOfEnumerable(property));
        }

        //If there is no fix tag attribute, we expand custom types and enumerated types with repeating group attribute
        if(fixTagAttribute is null)
        {
          if(!isEnumerable)
          {
            innerType = property.PropertyType;
            //Avoid recursion for obvious built in types.
            if(!AllowedType(innerType))
            {
              parents.Push(CreateParentNode(property));
              Map(innerType, parents);
              parents.Pop();
            }
          }
          else if(isEnumerable && repeatingGroup != null)
          {
            //Finding the tpye of enumeration
            innerType = GetInnerTypeOfEnumerable(property);
            if(!AllowedType(innerType))
            {
              parents.Push(CreateRepeatingParentNode(property, repeatingGroup, innerType));
              Map(innerType, parents);
              parents.Pop();
            }
          }
        }
        else if(fixTagAttribute != null)
        {
          var typeConverter = property.GetCustomAttribute<TypeConverterAttribute>();
          TagMapLeaf value;
          //Enumerable: string[] can have a FixTag for repeating groups
          if(isEnumerable && repeatingGroup != null)
          {
            //Finding the tpye of enumeration
            innerType = GetInnerTypeOfEnumerable(property);
            if(AllowedType(innerType) || typeConverter != null)
            {
              value = AddEnumerableLeaf(parents, property, fixTagAttribute, repeatingGroup, typeConverter, fixTagAttribute.Tag, innerType);
            }
            else
            {
              throw new NotSupportedException("FixTagAttribute on enumerable must by primitive or string typed.");
            }
          }
          else if(!isEnumerable)
          {
            //TODO: Nullable<Types>, IsValueType, IsSerializable
            innerType = property.PropertyType;
            if(AllowedType(innerType)
              || AllowedType(GetInnerTypeOfNullable(property, typeof(Nullable<>)))
              || typeConverter != null)
            {
              value = AddLeafNode(parents, property, fixTagAttribute, typeConverter, fixTagAttribute.Tag);
            }
            else
            {
              throw new NotSupportedException("FixTagAttribute must have a TypeConverter or be a primitive or string typed.");
            }
          }
        }
      }

    }

    private bool AllowedType(Type type)
    {
      return type != null && (type.IsPrimitive || type == typeof(string) || type == typeof(DateTimeOffset));
    }

    private TagMapNode CreateParentNode(PropertyInfo property)
    {
      return new TagMapNode() { Current = property };
    }

    private TagMapNode CreateRepeatingParentNode(PropertyInfo property, RepeatingGroupAttribute repeatingGroup, Type innerType)
    {
      return TagMapNode.CreateEnumerable<TagMapNode>(property, repeatingGroup.Tag, innerType);
    }

    private TagMapLeaf AddLeafNode(Stack<TagMapNode> parents, PropertyInfo property, FixTagAttribute fixTagAttribute, TypeConverterAttribute typeConverter, int key)
    {
      TagMapLeaf value = new TagMapLeaf()
      {
        Current = property,
        Parents = parents.ToList(),
        TypeConverterName = typeConverter?.ConverterTypeName,
        IsEncoded = fixTagAttribute.Encoded
      };
      _map.TryAdd(key, value);
      return value;
    }

    private TagMapLeaf AddEnumerableLeaf(Stack<TagMapNode> parents, PropertyInfo property, FixTagAttribute fixTagAttribute, RepeatingGroupAttribute repeatingGroup, TypeConverterAttribute typeConverter, int key, Type innerType)
    {
      var value = TagMapNode.CreateEnumerable<TagMapLeaf>(property, repeatingGroup.Tag, innerType);
      value.Parents = parents.ToList();
      value.TypeConverterName = typeConverter?.ConverterTypeName;
      value.IsEncoded = fixTagAttribute.Encoded;
      _map.TryAdd(key, value);
      return value;
    }

    private void AddRepeatingGroupLeaf(Stack<TagMapNode> parents, PropertyInfo property, RepeatingGroupAttribute repeatingGroup, Type innerType)
    {
      var value = TagMapLeaf.CreateRepeatingTag<TagMapLeaf>(property, innerType);
      value.Parents = parents.ToList();
      value.IsEncoded = false;
      value.TypeConverterName = null;
      _map.TryAdd(repeatingGroup.Tag, value);
    }

    private Type GetInnerTypeOfEnumerable(PropertyInfo property)
    {
      Type innerType = property.PropertyType.GetInterfaces()
         .Where(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IEnumerable<>))
         .Select(t => t.GetGenericArguments()[0]).FirstOrDefault();
      if(innerType is null)
      {
        innerType = property.PropertyType.GetGenericArguments()[0];
      }

      return innerType;
    }

    private Type GetInnerTypeOfNullable(PropertyInfo property, Type typeOf)
    {
      if(property.PropertyType.IsGenericType
        && property.PropertyType.GetGenericTypeDefinition() == typeOf)
      {
        return property.PropertyType.GetGenericArguments()[0];
      }

      return null;
    }

    private int GetTypeKey(ReadOnlySpan<byte> bytes)
    {
      int result = 1;
      foreach(var b in bytes)
      {
        result *= b;
      }
      return result;
    }
  }
}
