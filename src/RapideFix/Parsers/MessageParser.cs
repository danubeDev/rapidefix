using System;
using RapideFix.Business;
using RapideFix.Business.Data;
using RapideFix.DataTypes;
using RapideFix.Extensions;
using RapideFix.Validation;

namespace RapideFix.Parsers
{
  public class MessageParser : IMessageParser<object>
  {
    private readonly ITagToPropertyMapper _propertyMapper;
    private readonly IPropertySetter _compositeSetter;
    private readonly IValidator _validators;
    private readonly MessageParserOptions _options;
    private readonly IObjectPool<FixMessageContext> _messageContextPool;

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
      _messageContextPool = new ObjectPool<FixMessageContext>(() => new FixMessageContext(), x => x.Reset());
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
      using(PooledObject<FixMessageContext> pooledMsgContext = _messageContextPool.Rent().Init(message, _options.Encoding))
      {
        if(!_validators.IsValid(message, pooledMsgContext.Value))
        {
          if(_options.ThrowIfInvalidMessage)
          {
            throw new ArgumentException("Invalid message");
          }
          return default;
        }
        return (T)Parse(message, pooledMsgContext.Value, targetObject);
      }
    }

    public object Parse(ReadOnlySpan<byte> message)
    {
      using(PooledObject<FixMessageContext> pooledMsgContext = _messageContextPool.Rent().Init(message, _options.Encoding))
      {
        if(!_validators.IsValid(message, pooledMsgContext.Value))
        {
          if(_options.ThrowIfInvalidMessage)
          {
            throw new ArgumentException("Invalid message");
          }
          return default;
        }

        var messageTypeStart = pooledMsgContext.Value.MessageTypeTagStartIndex;
        var lengthOfMessageType = message.Slice(messageTypeStart + 1).IndexOf(Constants.SOHByte) - KnownFixTags.MessageType.Length + 1;

        var messageTypeValueStart = messageTypeStart + KnownFixTags.MessageType.Length;

        Type targetObjectType = _propertyMapper.TryGetMessageType(
          message.Slice(messageTypeValueStart, lengthOfMessageType));
        object targetObject = Activator.CreateInstance(targetObjectType);

        return Parse(message, pooledMsgContext.Value, targetObject);
      }
    }

    public object Parse(ReadOnlySpan<byte> message, object targetObject)
    {
      if(targetObject == null)
      {
        throw new ArgumentNullException(nameof(targetObject));
      }
      using(PooledObject<FixMessageContext> pooledMsgContext = _messageContextPool.Rent().Init(message, _options.Encoding))
      {
        if(!_validators.IsValid(message, pooledMsgContext.Value))
        {
          if(_options.ThrowIfInvalidMessage)
          {
            throw new ArgumentException("Invalid message");
          }
          return default;
        }
        return Parse(message, pooledMsgContext.Value, targetObject);
      }
    }

    public object Parse(ReadOnlyMemory<byte> message, Func<ReadOnlyMemory<byte>, object> targetObjectFactory)
    {
      using(PooledObject<FixMessageContext> pooledMsgContext = _messageContextPool.Rent().Init(message.Span))
      {
        if(!_validators.IsValid(message.Span, pooledMsgContext.Value))
        {
          if(_options.ThrowIfInvalidMessage)
          {
            throw new ArgumentException("Invalid message");
          }
          return default;
        }
        object targetObject = targetObjectFactory(message);

        return Parse(message.Span, pooledMsgContext.Value, targetObject);
      }
    }

    public T Parse<T>(ReadOnlyMemory<byte> message, Func<ReadOnlyMemory<byte>, T> targetObjectFactory)
    {
      using(PooledObject<FixMessageContext> pooledMsgContext = _messageContextPool.Rent().Init(message.Span))
      {
        if(!_validators.IsValid(message.Span, pooledMsgContext.Value))
        {
          if(_options.ThrowIfInvalidMessage)
          {
            throw new ArgumentException("Invalid message");
          }
          return default;
        }
        T targetObject = targetObjectFactory(message);

        return (T)Parse(message.Span, pooledMsgContext.Value, targetObject);
      }
    }

    private object Parse(ReadOnlySpan<byte> message, FixMessageContext messageContext, object targetObject)
    {
      ReadOnlySpan<byte> messagePart = message.Slice(messageContext.MessageTypeTagStartIndex + 1);
      int indexSOH = 0;
      int indexEquals = 0;
      while(indexSOH <= messagePart.Length)
      {
        indexEquals = messagePart.IndexOf(Constants.EqualsByte);
        if(indexEquals <= 0)
        {
          break;
        }
        var propertyLeaf = _propertyMapper.TryGet(messagePart.Slice(0, indexEquals));
        indexSOH = messagePart.IndexOf(Constants.SOHByte);
        if(propertyLeaf != null)
        {
          if(indexSOH <= 0)
          {
            break;
          }
          indexEquals = indexEquals + 1;
          var valueSlice = messagePart.Slice(indexEquals, indexSOH - indexEquals);
          _compositeSetter.Set(valueSlice, propertyLeaf, messageContext, targetObject);
        }
        messagePart = messagePart.Slice(indexSOH + 1);
      }
      return targetObject;
    }

  }
}
