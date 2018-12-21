using System;
using RapideFix.Business;
using RapideFix.Business.Data;
using RapideFix.Parsers;
using RapideFix.Validation;

namespace RapideFix.ParserBuilders
{
  /// <summary>
  /// This class builds a message parser that is capable of returning <c>TOutput</c> from input data.
  /// </summary>
  /// <typeparam name="TOutput">Output type of the message parser built.</typeparam>
  public class ParserBuilder<TOutput>
  {
    private IValidator _validator;
    private ITagToPropertyMapper _propertyMapper;
    private ITypedPropertySetter _propertySetter;
    private ISubPropertySetterFactory _subPropertySetterFactory;
    private MessageParserOptions _options;

    /// <summary>
    /// Sets a property mapper object to configure the <see cref="IMessageParser"/> instances built by ParserBuilder.
    /// </summary>
    /// <returns>The builder instance.</returns>
    public ParserBuilder<TOutput> SetPropertyMapper(ITagToPropertyMapper propertyMapper)
    {
      if(_propertyMapper != null)
      {
        throw new ArgumentException("Property Mapper is already set");
      }
      _propertyMapper = propertyMapper ?? throw new ArgumentNullException(nameof(propertyMapper));
      return this;
    }

    /// <summary>
    /// Sets additional types to be mapped by the property mapper.
    /// </summary>
    /// <returns>The builder instance.</returns>
    public ParserBuilder<TOutput> AddOutputType<TAdditionalOutputType>()
    {
      if(_propertyMapper == null)
      {
        _propertyMapper = new TagToPropertyMapper();
      }
      _propertyMapper.Map<TAdditionalOutputType>();
      return this;
    }

    /// <summary>
    /// Sets a property setter object to configure the <see cref="IMessageParser"/> instances built by ParserBuilder.
    /// </summary>
    /// <returns>The builder instance.</returns>
    public ParserBuilder<TOutput> SetPropertySetter(ITypedPropertySetter propertySetter)
    {
      if(_propertySetter != null)
      {
        throw new ArgumentException("Property Setter is already set");
      }
      _propertySetter = propertySetter ?? throw new ArgumentNullException(nameof(propertySetter));
      return this;
    }

    /// <summary>
    /// Sets a sub property setter factory instance to override default logic of setting property values.
    /// </summary>
    /// <returns>The builder instance.</returns>
    public ParserBuilder<TOutput> SetSubPropertySetter(ISubPropertySetterFactory subPropertySetterFactory)
    {
      if(_subPropertySetterFactory != null)
      {
        throw new ArgumentException("SubProperty setter is already set");
      }
      _subPropertySetterFactory = subPropertySetterFactory ?? throw new ArgumentNullException(nameof(subPropertySetterFactory));
      return this;
    }

    /// <summary>
    /// Sets a validator collection to configure the <see cref="IMessageParser"/> instances built by ParserBuilder.
    /// </summary>
    /// <returns>The builder instance.</returns>
    public ParserBuilder<TOutput> SetValidators(IValidator validatorCollection)
    {
      if(_validator != null)
      {
        throw new ArgumentException("Validators are already set");
      }
      _validator = validatorCollection ?? throw new ArgumentNullException(nameof(validatorCollection));
      return this;
    }

    /// <summary>
    /// Sets message parsing options to configure the <see cref="IMessageParser"/> instances built by ParserBuilder.
    /// </summary>
    /// <returns>The builder instance.</returns>
    public ParserBuilder<TOutput> SetOptions(MessageParserOptions options)
    {
      if(_options != null)
      {
        throw new ArgumentException("Options already set");
      }
      _options = options ?? throw new ArgumentNullException(nameof(options));
      return this;
    }

    /// <summary>
    /// Creates an IMessageParser object.
    /// </summary>
    /// <typeparam name="TInput">The input type of the data.</typeparam>
    /// <returns>A message parser.</returns>
    public IMessageParser<TOutput, TInput> Build<TInput>() where TInput : struct
    {
      var propertyMapper = _propertyMapper ?? new TagToPropertyMapper();
      var subPropertySetterFactory = _subPropertySetterFactory ?? new SubPropertySetterFactory();
      var propertySetter = _propertySetter ?? new CompositePropertySetter(subPropertySetterFactory);
      var validator = _validator ?? new ValidatorCollection(IntegerToFixConverter.Instance);
      var options = _options ?? new MessageParserOptions();
      return Build<TInput>(propertyMapper, propertySetter, validator, options);
    }

    /// <summary>
    /// Creates an IMessageParser object.
    /// </summary>
    /// <typeparam name="TInput">The input type of the data.</typeparam>
    /// <param name="options">Custom <c>MessageParserOptions</c> to this instance of the IMessageParser built.</param>
    /// <returns>A message parser.</returns>
    public IMessageParser<TOutput, TInput> Build<TInput>(MessageParserOptions options) where TInput : struct
    {
      var propertyMapper = _propertyMapper ?? new TagToPropertyMapper();
      var subPropertySetterFactory = _subPropertySetterFactory ?? new SubPropertySetterFactory();
      var propertySetter = _propertySetter ?? new CompositePropertySetter(subPropertySetterFactory);
      var validator = _validator ?? new ValidatorCollection(IntegerToFixConverter.Instance);
      return Build<TInput>(propertyMapper, propertySetter, validator, options);
    }

    /// <summary>
    /// Creates an IMessageParser object.
    /// </summary>
    /// <typeparam name="TInput">The input type of the data.</typeparam>
    /// <param name="propertyMapper">Custom <c>ITagToPropertyMapper</c> to this instance of the IMessageParser built.</param>
    /// <returns>A message parser.</returns>
    public IMessageParser<TOutput, TInput> Build<TInput>(ITagToPropertyMapper propertyMapper) where TInput : struct
    {
      var subPropertySetterFactory = _subPropertySetterFactory ?? new SubPropertySetterFactory();
      var propertySetter = _propertySetter ?? new CompositePropertySetter(subPropertySetterFactory);
      var validator = _validator ?? new ValidatorCollection(IntegerToFixConverter.Instance);
      var options = _options ?? new MessageParserOptions();
      return Build<TInput>(propertyMapper, propertySetter, validator, options);
    }

    /// <summary>
    /// Creates an IMessageParser object.
    /// </summary>
    /// <typeparam name="TInput">The input type of the data.</typeparam>
    /// <param name="propertyMapper">Custom <c>ITagToPropertyMapper</c> to this instance of the IMessageParser built.</param>
    /// <param name="propertySetter">Custom <c>ITypedPropertySetter</c> to this instance of the IMessageParser built.</param>
    /// <param name="validatorCollection">Custom <c>IValidator</c> to this instance of the IMessageParser built.</param>
    /// <param name="options">Custom <c>MessageParserOptions</c> to this instance of the IMessageParser built.</param>
    /// <returns>A message parser.</returns>
    public IMessageParser<TOutput, TInput> Build<TInput>(ITagToPropertyMapper propertyMapper,
      ITypedPropertySetter propertySetter, IValidator validatorCollection, MessageParserOptions options)
    {
      if(propertyMapper == null)
      {
        throw new ArgumentNullException(nameof(propertyMapper));
      }
      if(propertySetter == null)
      {
        throw new ArgumentNullException(nameof(propertySetter));
      }
      if(validatorCollection == null)
      {
        throw new ArgumentNullException(nameof(validatorCollection));
      }
      if(options == null)
      {
        throw new ArgumentNullException(nameof(options));
      }

      propertyMapper.Map<TOutput>();
      if(typeof(TInput) == typeof(char))
      {
        return new TypedStringMessageParser<TOutput>(propertyMapper, propertySetter, validatorCollection, options)
          as IMessageParser<TOutput, TInput>;
      }

      if(typeof(TInput) == typeof(byte))
      {
        if(typeof(TOutput) == typeof(object))
        {
          return new MessageParser(propertyMapper, propertySetter, validatorCollection, options)
            as IMessageParser<TOutput, TInput>;
        }
        else
        {
          return new TypedMessageParser<TOutput>(propertyMapper, propertySetter, validatorCollection, options)
            as IMessageParser<TOutput, TInput>;
        }
      }
      throw new NotSupportedException("Input type is not supported");
    }

  }
}
