## Stream Parsers

This article introduces the context of streamed parsers. The goal of a stream parser to process a continuous stream of input data into one or more parsed messages.

Streamed parsers are Observable. To receive the messages an observer must subscribe to the parser. Once a parsed message is available the observable will push it to the subscribed observers.

There are two type of streamed parsers available in the library, the ```StreamParser``` and the ```PipeParser```. Both using the [System.IO.Pipelines](https://docs.microsoft.com/en-us/dotnet/api/system.io.pipelines
) in their implementation. Usage of the two parsers are very similar, hence this article is introducing it through the ```PipeParser```.

>Note, that stream parsers are designed to return the same type of messages.

### PipeParser

The public interface of a ```PipeParser``` is simple. In the constructor it is given a ```Pipe``` or a ```PipeReader```, an ```IMessageParser<T, byte>``` to parse messages from bytes to type of *T*, and a *SupportedFixVersion* to indicate that each message must support a given fix version.

Optionally, a ```Func<ReadOnlyMemory<byte>, T>``` function can be passed to let the user create the target object given the input.

```PipeParser``` provides a *Subscribe* method for its observables, and a *ListenAsync* to start listening the input stream of data. ListenAsync will complete once the input has ended or cancellation has been triggered.

The example below shows how to process data with the ```PipeParser```:

```csharp
// Construct the pipe
var pipe = new Pipe();

// Create a message parser that can parser bytes into Order
var messageParser = new ParserBuilder<Order>().Build<byte>(newMessageParserOptions() { ThrowIfInvalidMessage = false });

// Create the piped parser, providing the pipe and an IMessageParser
var parser = new PipeParser<Order>(pipe.Reader, messageParser,SupportedFixVersion.Fix44);

// We subscribe to the observable to print the parsed messages
parser.Subscribe(
  order => Console.WriteLine($"Order {order.Symbol} - Px{order.Price}, Qty {order.Quantity}"),
  ex => Console.WriteLine($"Error: {ex.Message}"));
  
// Start observing and await until all messages observed
await parser.ListenAsync();
```



