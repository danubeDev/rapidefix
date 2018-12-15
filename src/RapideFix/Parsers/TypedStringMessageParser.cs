using System;
using System.Text;
using RapideFix.Business;
using RapideFix.Business.Data;
using RapideFix.DataTypes;
using RapideFix.Extensions;
using RapideFix.Validation;

namespace RapideFix.Parsers
{
  public class TypedStringMessageParser<TTarget> : IMessageParser<TTarget, char>
  {
    private readonly ITagToPropertyMapper _propertyMapper;
    private readonly ITypedPropertySetter _typedPropertySetter;
    private readonly StringMessageParserOptions _options;
    private readonly FixMessageContext _messageContext;
    private readonly bool _isValueType;

    public TypedStringMessageParser(
      ITagToPropertyMapper tagToPropertyMapper,
      ITypedPropertySetter typedPropertySetter,
      IValidator validators,
      StringMessageParserOptions options
      )
    {
      _propertyMapper = tagToPropertyMapper ?? throw new ArgumentNullException(nameof(tagToPropertyMapper));
      _typedPropertySetter = typedPropertySetter ?? throw new ArgumentNullException(nameof(typedPropertySetter));
      _options = options ?? throw new ArgumentNullException(nameof(options));
      _messageContext = new FixMessageContext();
      _propertyMapper.Map<TTarget>();
      _isValueType = typeof(TTarget).IsValueType;
    }

    public TTarget Parse(ReadOnlySpan<char> message)
    {
      _messageContext.Setup(message);

      TTarget targetObject;
      if(_isValueType)
      {
        targetObject = default;
      }
      else
      {
        targetObject = Activator.CreateInstance<TTarget>();
      }
      return Parse(message, _messageContext, ref targetObject);
    }

    public TTarget Parse(ReadOnlyMemory<char> message, Func<ReadOnlyMemory<char>, TTarget> targetObjectFactory)
    {
      _messageContext.Setup(message.Span);
      TTarget targetObject = targetObjectFactory(message);
      return Parse(message.Span, _messageContext, ref targetObject);
    }

    private ref readonly TTarget Parse(ReadOnlySpan<char> message, FixMessageContext messageContext, ref TTarget targetObject)
    {
      ReadOnlySpan<char> messagePart = message;
      int indexSOH = 0;
      int indexEquals = 0;
      while(messagePart.Length > 0)
      {
        if(TraverseMessageBody(messagePart, out indexEquals, out indexSOH))
        {
          if(_propertyMapper.TryGet(messagePart.Slice(0, indexEquals), out var propertyLeaf))
          {
            indexEquals++;
            var valueSlice = messagePart.Slice(indexEquals, indexSOH - indexEquals);
            if(_isValueType)
            {
              _typedPropertySetter.SetTarget(valueSlice, propertyLeaf, messageContext, ref targetObject);
            }
            else
            {
              _typedPropertySetter.Set(valueSlice, propertyLeaf, messageContext, targetObject);
            }
          }
        }
        else
        {
          break;
        }
        messagePart = messagePart.Slice(indexSOH + 1);
      }
      return ref targetObject;
    }

    private bool TraverseMessageBody(ReadOnlySpan<char> messagePart, out int indexEquals, out int indexSOH)
    {
      indexEquals = -1;
      indexSOH = -1;
      int i;
      for(i = 0; i < messagePart.Length; i++)
      {
        if(messagePart[i] == Constants.Equal)
        {
          indexEquals = i;
          break;
        }
      }
      for(i++; i < messagePart.Length; i++)
      {
        if(messagePart[i] == _options.SOHChar)
        {
          indexSOH = i;
          break;
        }
      }
      return indexEquals != -1 && indexSOH != -1;
    }

  }
}
