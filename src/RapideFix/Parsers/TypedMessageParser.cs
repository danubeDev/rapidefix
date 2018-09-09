using System;
using RapideFix.Business;
using RapideFix.Business.Data;
using RapideFix.DataTypes;
using RapideFix.Extensions;
using RapideFix.Validation;

namespace RapideFix.Parsers
{
  public class TypedMessageParser<TTarget> : IMessageParser<TTarget>
  {
    private readonly ITagToPropertyMapper _propertyMapper;
    private readonly ITypedPropertySetter _typedPropertySetter;
    private readonly IValidator _validators;
    private readonly MessageParserOptions _options;
    private readonly IObjectPool<FixMessageContext> _messageContextPool;

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
      _messageContextPool = new ObjectPool<FixMessageContext>(() => new FixMessageContext(), x => x.Reset());
    }

    public TTarget Parse(ReadOnlySpan<byte> message)
    {
      using(PooledObject<FixMessageContext> pooledMsgContext = _messageContextPool.Rent().Init(message))
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
        if(targetObjectType != typeof(TTarget))
        {
          throw new InvalidCastException("MessageType on type does not match TTarget");
        }
        TTarget targetObject = Activator.CreateInstance<TTarget>();
        return Parse(message, pooledMsgContext.Value, ref targetObject);
      }
    }

    public TTarget Parse(ReadOnlyMemory<byte> message, Func<ReadOnlyMemory<byte>, TTarget> targetObjectFactory)
    {
      using(PooledObject<FixMessageContext> pooledMsgContext = _messageContextPool.Rent().Init(message.Span, _options.Encoding))
      {
        if(!_validators.IsValid(message.Span, pooledMsgContext.Value))
        {
          if(_options.ThrowIfInvalidMessage)
          {
            throw new ArgumentException("Invalid message");
          }
          return default;
        }
        TTarget targetObject = targetObjectFactory(message);
        return Parse(message.Span, pooledMsgContext.Value, ref targetObject);
      }
    }

    private ref readonly TTarget Parse(ReadOnlySpan<byte> message, FixMessageContext messageContext, ref TTarget targetObject)
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
          _typedPropertySetter.SetTarget(valueSlice, propertyLeaf, messageContext, ref targetObject);
        }
        messagePart = messagePart.Slice(indexSOH + 1);
      }
      return ref targetObject;
    }

  }
}
