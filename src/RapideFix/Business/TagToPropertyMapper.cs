using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using RapideFix.Attributes;
using RapideFix.Business.Data;

namespace RapideFix.Business
{
  public class TagToPropertyMapper : ITagToPropertyMapper
  {
    private Dictionary<int, TagMapLeaf> _map = new Dictionary<int, TagMapLeaf>();

    public TagMapLeaf Get(byte[] tag)
    {
      int key = IntegerToFixConverter.Instance.ConvertBack(tag);
      return _map[key];
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
        if(fixTagAttribute == null)
        {
          if(!isEnumerable)
          {
            innerType = property.PropertyType;
            //Avoid recursion for obvious built in types.
            if(!innerType.IsPrimitive && innerType != typeof(string))
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
            if(!innerType.IsPrimitive && innerType != typeof(string))
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
            if(innerType.IsPrimitive || innerType == typeof(string) || typeConverter != null)
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
            innerType = property.PropertyType;
            if(innerType.IsPrimitive || innerType == typeof(string) || typeConverter != null)
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

    private TagMapNode CreateParentNode(PropertyInfo property)
    {
      return new TagMapNode() { Current = property };
    }

    private TagMapNode CreateRepeatingParentNode(PropertyInfo property, RepeatingGroupAttribute repeatingGroup, Type innerType)
    {
      return new EnumerableTagMapNode()
      {
        Current = property,
        InnerType = innerType,
        RepeatingTagNumber = repeatingGroup.Tag
      };
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
      _map.Add(key, value);
      return value;
    }

    private EnumerableTagMapLeaf AddEnumerableLeaf(Stack<TagMapNode> parents, PropertyInfo property, FixTagAttribute fixTagAttribute, RepeatingGroupAttribute repeatingGroup, TypeConverterAttribute typeConverter, int key, Type innerType)
    {
      EnumerableTagMapLeaf value = new EnumerableTagMapLeaf()
      {
        Current = property,
        Parents = parents.ToList(),
        TypeConverterName = typeConverter?.ConverterTypeName,
        IsEncoded = fixTagAttribute.Encoded,
        RepeatingTagNumber = repeatingGroup.Tag,
        InnerType = innerType
      };

      _map.Add(key, value);
      return value;
    }

    private void AddRepeatingGroupLeaf(Stack<TagMapNode> parents, PropertyInfo property, RepeatingGroupAttribute repeatingGroup, Type innerType)
    {
      var value = new RepeatingGroupTagMapLeaf()
      {
        Current = property,
        Parents = parents.ToList(),
        InnerType = innerType,
        IsEncoded = false,
        TypeConverterName = null
      };
      _map.Add(repeatingGroup.Tag, value);
    }

    private Type GetInnerTypeOfEnumerable(PropertyInfo property)
    {
      Type innerType = property.PropertyType.GetInterfaces()
         .Where(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IEnumerable<>))
         .Select(t => t.GetGenericArguments()[0]).FirstOrDefault();
      if(innerType == null)
      {
        innerType = property.PropertyType.GetGenericArguments()[0];
      }

      return innerType;
    }
  }
}
