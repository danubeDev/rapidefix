using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using RapideFix.Attributes;
using RapideFix.Extensions;

namespace RapideFix.Business
{
  public class TagToPropertyMapper
  {
    public class TagMapNode
    {
      public PropertyInfo Current { get; set; }

      public bool IsRepeating { get; set; }

      public byte[] RepeatingTag { get; set; }
    }

    public class TagMapLeaf : TagMapNode
    {
      public IList<TagMapNode> Parents { get; set; }

      public string TypeConverterName { get; set; }

      public bool IsEncoded { get; set; }
    }

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
          AddRepeatingGroup(parents, property, repeatingGroup);
        }

        //If there is no fix tag attribute, we expand custom types, and enumerated types with repeating group attribute
        if(fixTagAttribute == null)
        {
          if(!isEnumerable)
          {
            innerType = property.PropertyType;
            //Avoid recursion for obvious built in types.
            if(!innerType.IsPrimitive && innerType != typeof(string))
            {
              parents.Push(new TagMapNode() { Current = property, IsRepeating = false });
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
              parents.Push(new TagMapNode() { Current = property, IsRepeating = true, RepeatingTag = repeatingGroup.Tag.ToKnownTag() });
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
              value = AddEnumerable(parents, property, fixTagAttribute, repeatingGroup, typeConverter, fixTagAttribute.Tag);
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
              value = AddSingle(parents, property, fixTagAttribute, typeConverter, fixTagAttribute.Tag);
            }
            else
            {
              throw new NotSupportedException("FixTagAttribute must have a TypeConverter or be a primitive or string typed.");
            }
          }
        }
      }

    }

    private TagMapLeaf AddSingle(Stack<TagMapNode> parents, PropertyInfo property, FixTagAttribute fixTagAttribute, TypeConverterAttribute typeConverter, int key)
    {
      TagMapLeaf value = new TagMapLeaf()
      {
        Current = property,
        Parents = parents.ToList(),
        IsEncoded = fixTagAttribute.Encoded,
        IsRepeating = false,
        TypeConverterName = typeConverter?.ConverterTypeName
      };
      _map.Add(key, value);
      return value;
    }

    private TagMapLeaf AddEnumerable(Stack<TagMapNode> parents, PropertyInfo property, FixTagAttribute fixTagAttribute, RepeatingGroupAttribute repeatingGroup, TypeConverterAttribute typeConverter, int key)
    {
      TagMapLeaf value = new TagMapLeaf()
      {
        Current = property,
        Parents = parents.ToList(),
        IsEncoded = fixTagAttribute.Encoded,
        IsRepeating = true,
        RepeatingTag = repeatingGroup.Tag.ToKnownTag(),
        TypeConverterName = typeConverter?.ConverterTypeName
      };
      _map.Add(key, value);
      return value;
    }

    private void AddRepeatingGroup(Stack<TagMapNode> parents, PropertyInfo property, RepeatingGroupAttribute repeatingGroup)
    {
      var value = new TagMapLeaf()
      {
        Current = property,
        Parents = parents.ToList(),
        IsRepeating = false,
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
