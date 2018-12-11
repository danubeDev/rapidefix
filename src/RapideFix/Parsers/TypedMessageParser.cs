using System;
using RapideFix.Business;
using RapideFix.Business.Data;
using RapideFix.DataTypes;
using RapideFix.Extensions;
using RapideFix.Validation;

namespace RapideFix.Parsers
{
  public class TypedMessageParser<TTarget> : IMessageParser<TTarget, byte>
  {
    private readonly ITagToPropertyMapper _propertyMapper;
    private readonly ITypedPropertySetter _typedPropertySetter;
    private readonly IValidator _validators;
    private readonly MessageParserOptions _options;
    private readonly FixMessageContext _messageContext;
    private readonly bool _isValueType;

    public TypedMessageParser(
      ITagToPropertyMapper tagToPropertyMapper,
      ITypedPropertySetter typedPropertySetter,
      IValidator validators,
      MessageParserOptions options
      )
    {
      _propertyMapper = tagToPropertyMapper ?? throw new ArgumentNullException(nameof(tagToPropertyMapper));
      _typedPropertySetter = typedPropertySetter ?? throw new ArgumentNullException(nameof(typedPropertySetter));
      _validators = validators ?? throw new ArgumentNullException(nameof(validators));
      _options = options ?? throw new ArgumentNullException(nameof(options));
      _messageContext = new FixMessageContext();
      _propertyMapper.Map<TTarget>();
      _isValueType = typeof(TTarget).IsValueType;
    }

    public TTarget Parse(ReadOnlySpan<byte> message)
    {
      _messageContext.Setup(message, _options.Encoding);
      if(!_validators.PreValidate(message, _messageContext))
      {
        if(_options.ThrowIfInvalidMessage)
        {
          throw new ArgumentException("Invalid message");
        }
        return default;
      }

      var messageTypeStart = _messageContext.MessageTypeTagStartIndex;
      var messageTypeEnd = message.Slice(messageTypeStart + 1).IndexOf(Constants.SOHByte);
      var lengthOfMessageType = messageTypeEnd - KnownFixTags.MessageType.Length + 1;

      var messageTypeValueStart = messageTypeStart + KnownFixTags.MessageType.Length;

      Type targetObjectType = _propertyMapper.TryGetMessageType(
        message.Slice(messageTypeValueStart, lengthOfMessageType));
      if(targetObjectType != typeof(TTarget))
      {
        throw new InvalidCastException("MessageType on type does not match TTarget");
      }

      TTarget targetObject;
      if(_isValueType)
      {
        targetObject = default;
      }
      else
      {
        targetObject = Activator.CreateInstance<TTarget>();
      }
      return Parse(message.Slice(messageTypeEnd + 1), _messageContext, ref targetObject);
    }

    public TTarget Parse(ReadOnlyMemory<byte> message, Func<ReadOnlyMemory<byte>, TTarget> targetObjectFactory)
    {
      _messageContext.Setup(message.Span, _options.Encoding);
      if(!_validators.PreValidate(message.Span, _messageContext))
      {
        if(_options.ThrowIfInvalidMessage)
        {
          throw new ArgumentException("Invalid message");
        }
        return default;
      }
      TTarget targetObject = targetObjectFactory(message);
      return Parse(message.Span, _messageContext, ref targetObject);
    }

    private ref readonly TTarget Parse(ReadOnlySpan<byte> message, FixMessageContext messageContext, ref TTarget targetObject)
    {
      int checksumLength = message.Length - messageContext.ChecksumTagStartIndex - 1;
      ReadOnlySpan<byte> messagePart = message;
      int indexSOH = 0;
      int indexEquals = 0;
      byte checksumValue = 0;
      while(messagePart.Length > checksumLength)
      {
        if(TraverseMessageBody(messagePart, out indexEquals, out indexSOH, ref checksumValue))
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
      messageContext.ChecksumValue = checksumValue;
      _validators.PostValidate(message, messageContext);
      return ref targetObject;
    }

    private bool TraverseMessageBody(ReadOnlySpan<byte> messagePart, out int indexEquals, out int indexSOH, ref byte checksumValue)
    {
      indexEquals = -1;
      indexSOH = -1;
      byte checksum = 0;
      int i;
      for(i = 0; i < messagePart.Length; i++)
      {
        checksum += messagePart[i];
        if(messagePart[i] == Constants.EqualsByte)
        {
          indexEquals = i;
          break;
        }
      }
      for(i++; i < messagePart.Length; i++)
      {
        checksum += messagePart[i];
        if(messagePart[i] == Constants.SOHByte)
        {
          indexSOH = i;
          break;
        }
      }
      checksumValue += checksum;
      return indexEquals != -1 && indexSOH != -1;
    }

  }
}
