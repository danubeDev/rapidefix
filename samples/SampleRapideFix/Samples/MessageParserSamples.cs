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
  public class MessageParserSamples
  {
    public void ParserByParserBuilder()
    {
      byte[] message = new MessageBuilder().AddRaw("35=D|53=10|44=145|55=ABC|").Build();
      var parser = new ParserBuilder<object>().AddOutputType<Order>().Build<byte>() as MessageParser;

      Order order = parser.Parse<Order>(message);

      Console.WriteLine($"Order {order.Symbol} - Px {order.Price}, Qty {order.Quantity}");
    }

    public void ParserByNewParser()
    {
      byte[] message = new MessageBuilder().AddRaw("35=D|53=10|44=145|55=ABC|").Build();

      // Create a property mapper and map types to be parsed. CompositePropertySetter is a composite of sub property setters.
      var propertyMapper = new TagToPropertyMapper(new SubPropertySetterFactory());
      propertyMapper.Map<Order>();

      // Create a property setter
      var compositeSetter = new CompositePropertySetter();

      // Create a validator collection to have all default validators
      var validators = new ValidatorCollection(IntegerToFixConverter.Instance);

      // Passing empty options
      var options = new MessageParserOptions();

      // Create MessageParser, passing dependencies
      var parser = new MessageParser(propertyMapper, compositeSetter, validators, options);

      Order order = parser.Parse<Order>(message);
      Console.WriteLine($"Order {order.Symbol} - Px {order.Price}, Qty {order.Quantity}");
    }

    public void ParserMultipleMessageTypes()
    {
      byte[] message0 = new MessageBuilder().AddRaw("35=D|53=10|44=145|55=ABC|").Build();
      byte[] message1 = new MessageBuilder().AddRaw("35=S|53=1|44=120|55=ABC.S|").Build();

      // Registering both Slice and Order message types when building the message parser.
      var parser = new ParserBuilder<object>()
        .AddOutputType<Order>()
        .AddOutputType<Slice>()
        .Build<byte>() as MessageParser;

      Order order = parser.Parse<Order>(message0);
      Slice slice = parser.Parse<Slice>(message1);

      Console.WriteLine($"Order {order.Symbol} - Px {order.Price}, Qty {order.Quantity}");
      Console.WriteLine($"Slice {slice.Symbol} - Px {slice.Price}, Qty {slice.Quantity}");
    }
  }
}
