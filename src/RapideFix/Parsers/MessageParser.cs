using System;
using RapideFix.Business;
using RapideFix.Business.Data;
using RapideFix.DataTypes;
using RapideFix.Extensions;
using RapideFix.Validation;

namespace RapideFix.Parsers
{
  public class MessageParser : IMessageParser<object, byte>
  {
    private readonly ITagToPropertyMapper _propertyMapper;
    private readonly IPropertySetter _compositeSetter;
    private readonly IValidator _validators;
    private readonly MessageParserOptions _options;
    private readonly FixMessageContext _messageContext;

    public MessageParser(
      ITagToPropertyMapper tagToPropertyMapper,
      IPropertySetter compositeSetter,
      IValidator validators,
      MessageParserOptions options
      )
    {
      _propertyMapper = tagToPropertyMapper ?? throw new ArgumentNullException(nameof(tagToPropertyMapper));
      _compositeSetter = compositeSetter ?? throw new ArgumentNullException(nameof(compositeSetter));
      _validators = validators ?? throw new ArgumentNullException(nameof(validators));
      _options = options ?? throw new ArgumentNullException(nameof(options));
      _messageContext = new FixMessageContext();
    }

    public T Parse<T>(ReadOnlySpan<byte> message)
    {
      return (T)Parse(message);
    }

    public T Parse<T>(ReadOnlySpan<byte> message, T targetObject)
    {
      if(targetObject == null)
      {
        throw new ArgumentNullException(nameof(targetObject));
      }
      _messageContext.Setup(message, _options.Encoding);
      if(!_validators.PreValidate(message, _messageContext))
      {
        if(_options.ThrowIfInvalidMessage)
        {
          throw new ArgumentException("Invalid message");
        }
        return default;
      }
      return (T)Parse(message, _messageContext, targetObject);
    }

    public object Parse(ReadOnlySpan<byte> message)
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
      object targetObject = Activator.CreateInstance(targetObjectType);

      return Parse(message.Slice(messageTypeStart + messageTypeEnd - 1), _messageContext, targetObject);
    }

    public object Parse(ReadOnlySpan<byte> message, object targetObject)
    {
      if(targetObject == null)
      {
        throw new ArgumentNullException(nameof(targetObject));
      }
      _messageContext.Setup(message, _options.Encoding);
      if(!_validators.PreValidate(message, _messageContext))
      {
        if(_options.ThrowIfInvalidMessage)
        {
          throw new ArgumentException("Invalid message");
        }
        return default;
      }
      return Parse(message, _messageContext, targetObject);
    }

    public object Parse(ReadOnlyMemory<byte> message, Func<ReadOnlyMemory<byte>, object> targetObjectFactory)
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
      object targetObject = targetObjectFactory(message);

      return Parse(message.Span, _messageContext, targetObject);
    }

    public T Parse<T>(ReadOnlyMemory<byte> message, Func<ReadOnlyMemory<byte>, T> targetObjectFactory)
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
      T targetObject = targetObjectFactory(message);

      return (T)Parse(message.Span, _messageContext, targetObject);
    }

    private object Parse(ReadOnlySpan<byte> message, FixMessageContext messageContext, object targetObject)
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
            _compositeSetter.Set(valueSlice, propertyLeaf, messageContext, targetObject);
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
      return targetObject;
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
