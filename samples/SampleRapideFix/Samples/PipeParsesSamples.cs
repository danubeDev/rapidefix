using System;
using System.Collections.Generic;
using System.IO.Pipelines;
using System.Text;
using System.Threading.Tasks;
using RapideFix;
using RapideFix.Business.Data;
using RapideFix.MessageBuilders;
using RapideFix.ParserBuilders;
using RapideFix.Parsers;
using SampleRapideFix.Data;

namespace SampleRapideFix.Samples
{
  public class PipeParserSamples
  {
    public async Task ParseMultipleMessages()
    {
      // Construct the pipe
      var pipe = new Pipe();

      // Create a message parser that can parser bytes into Order
      var messageParser = new ParserBuilder<Order>().Build<byte>(new MessageParserOptions() { ThrowIfInvalidMessage = false });

      // Create the piped parser, providing the pipe and an IMessageParser
      var parser = new PipeParser<Order>(pipe.Reader, messageParser, SupportedFixVersion.Fix44);

      // We subscribe to the observable to print the parsed messages
      parser.Subscribe(
        order => Console.WriteLine($"Order {order.Symbol} - Px {order.Price}, Qty {order.Quantity}"),
        ex => Console.WriteLine($"Error: {ex.Message}"));

      // We create sample messages and write them into the pipe asnyc
      var inputTask = Task.Run(() => CreateSimulatedInput(pipe));

      // Start observing and await until all messages observed
      await parser.ListenAsync();

      // Should be completed by now
      await inputTask;
    }

    private async Task CreateSimulatedInput(Pipe pipe)
    {
      // Creating 10 messages and feeding into the pipe simulating an external source
      var messageBuilder = new MessageBuilder();
      for(int i = 0; i < 10; i++)
      {
        byte[] message = messageBuilder
          .AddBeginString(SupportedFixVersion.Fix44)
          .AddTag(35, "D")
          .AddTag(53, (i * 10).ToString())
          .AddTag(44, (100 + i).ToString())
          .AddTag(55, "ABC.L")
          .Build();
        await pipe.Writer.WriteAsync(message);
      }
      await pipe.Writer.FlushAsync();
      pipe.Writer.Complete();
    }
  }
}
