
## Getting started

### Installation

Create a new dotnet core 2.2 application and install [DanubeDev.RapideFix](https://www.nuget.org/packages/DanubeDev.RapideFix/) nuget package.
RapideFix classes and structs will be available under ```RapideFix``` default namespace.

### What to parse

RapideFix primary goal is to parse FIX messages into POCO classes or structs. For this reason, our data source can be a low-level data type like *byte* or *char*. As we do not parse individual bytes or chars, the parsers can work with ```ReadOnlySpan```'s or ```ReadOnlyMemory```'s of bytes or chars. This way we can easily parse a byte array or a string, as both have an extension method for [AsSpan()](https://docs.microsoft.com/en-us/dotnet/api/system.memoryextensions.asspan
).

In some use-cases we do not only want to parse a single message, but many messages. RapideFix provides a solution for this: *individual messages* or *data streams* can be parsed with the library. For individual messages we can simply use the [message parsers](./MessageParsers.md), and for stream we use the [stream parsers](./StreamParsers.md), both located under the Parsers namespace.

### How to parse

When parsing into POCO, RapideFix distinguishes two scenarios:
1. parsing all messages (stream or individual) into the same C# type
2. parsing different type messages into different C# types

In the first case the output type of the parser is the user selected type, while in the second case it is most like to be type of ```object```. As messages parser are generic to both input and output types, this is an important distinction between them.

### Build your first FixParser

To get started we build a really simple parser, which can parse a ```string``` into a struct. To build such a parser we can use the help of a ```ParserBuilder```. ParserBuilders can build multiple similar parsers using the same configuration.

#### Step 1. Add required usings

```csharp
using RapideFix.Business.Data;
using RapideFix.ParserBuilders;
using RapideFix.Parsers;
```

#### Step 2. Create a parser
In this step we can provide the SOH character, which is the delimiter char for the fix tags.

```csharp
IMessageParser<Quote, char> parser = new ParserBuilder<Quote>()
        .Build<char>(new MessageParserOptions() { SOHChar = '|' });
```

#### Step 3. Call the parser method
```csharp
Quote quote = parser.Parse(message);
```

#### The final example

```csharp
using System;
using RapideFix.Business.Data;
using RapideFix.ParserBuilders;
using RapideFix.Parsers;
using SampleRapideFix.Data;

namespace SampleRapideFix.Samples
{
  public class GettingStarted
  {
    public void ParserSample()
    {
      string message = "134=10|132=145|62=20190112-23:34:12|";
      IMessageParser<Quote, char> parser = new ParserBuilder<Quote>()
        .Build<char>(new MessageParserOptions() { SOHChar = '|' });

      Quote quote = parser.Parse(message);

      Console.WriteLine($"Quote Px {quote.Price}, Qty {quote.Quantity}, @{quote.Expiry.Hour}:{quote.Expiry.Minute}");
    }
  }
}
```

Notice that in the example above, we parse into ```Quote``` struct, which is a user defined type. This is defined in a separate file. Quote needs to mark its properties with ```FixTag``` attribute with the tag number:

```csharp
using System;
using RapideFix.Attributes;

namespace SampleRapideFix.Data
{
  public struct Quote
  {
    [FixTag(134)]
    public int Quantity { get; set; }

    [FixTag(132)]
    public double Price { get; set; }

    [FixTag(62)]
    public DateTimeOffset Expiry { get; set; }
  }
}
```

The output of the sample:
```
Quote Px 145, Qty 10, @23:34
```

### Next Steps

In the next steps take a look at creating custom [message parsers](./MessageParsers.md) or [stream parsers](./StreamParsers.md).

