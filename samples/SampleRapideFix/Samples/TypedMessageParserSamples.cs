using System;
using RapideFix;
using RapideFix.Business;
using RapideFix.Business.Data;
using RapideFix.MessageBuilders;
using RapideFix.ParserBuilders;
using RapideFix.Parsers;
using RapideFix.Validation;
using SampleRapideFix.Data;

namespace SampleRapideFix.Samples
{
  public class TypedMessageParserSamples
  {
    public void ParserByParserBuilder()
    {
      byte[] message = new MessageBuilder().AddRaw("35=D|53=10|44=145|55=ABC|").Build();
      IMessageParser<Order, byte> parser = new ParserBuilder<Order>().Build<byte>();

      Order order = parser.Parse(message);

      Console.WriteLine($"Order {order.Symbol} - Px {order.Price}, Qty {order.Quantity}");
    }

    public void ParserByNewParser()
    {
      byte[] message = new MessageBuilder().AddRaw("35=D|53=10|44=145|55=ABC|").Build();

      // Create a property mapper and map types to be parsed
      var propertyMapper = new TagToPropertyMapper();
      propertyMapper.Map<Order>();

      // Create a property setter. CompositePropertySetter is a composite of sub property setters
      var compositeSetter = new CompositePropertySetter(new SubPropertySetterFactory());

      // Create a validator collection to have all default validators
      var validators = new ValidatorCollection(IntegerToFixConverter.Instance);

      // Passing empty options
      var options = new MessageParserOptions();

      // Create TypedMessageParser, passing dependencies
      var parser = new TypedMessageParser<Order>(propertyMapper, compositeSetter, validators, options);

      Order order = parser.Parse(message);
      Console.WriteLine($"Order {order.Symbol} - Px {order.Price}, Qty {order.Quantity}");
    }

    public void ParserValueType()
    {
      byte[] message = new MessageBuilder().AddRaw("35=Q|134=10|132=145|62=20190112-23:34:12|").Build();
      IMessageParser<Quote, byte> parser = new ParserBuilder<Quote>().Build<byte>();

      Quote quote = parser.Parse(message);

      Console.WriteLine($"Quote Px {quote.Price}, Qty {quote.Quantity}");
    }
  }
}
