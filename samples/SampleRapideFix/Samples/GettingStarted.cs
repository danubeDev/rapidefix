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
