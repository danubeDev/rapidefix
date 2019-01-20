using System;
using System.Threading;
using System.Threading.Tasks;
using RapideFix;
using RapideFix.Business;
using RapideFix.MessageBuilders;
using RapideFix.Parsers;
using RapideFix.Validation;
using RapideFixFixture.TestTypes;
using SampleRapideFix.Samples;

namespace SampleRapideFix
{
  public class Program
  {
    public async static Task Main(string[] args)
    {
      new GettingStarted().ParserSample();
      new TypedStringMessageParserSamples().Parser();
      new MessageParserSamples().ParserByParserBuilder();
      new MessageParserSamples().ParserByNewParser();
      new MessageParserSamples().ParserMultipleMessageTypes();
      new TypedMessageParserSamples().ParserByParserBuilder();
      new TypedMessageParserSamples().ParserByNewParser();
      new TypedMessageParserSamples().ParserValueType();
      new MessageBuilderSamples().AddTag();
      new MessageBuilderSamples().AddTagRaw();
      new MessageBuilderSamples().AddFixVersion();
      new MessageBuilderSamples().BuildSpan();
      await new PipeParserSamples().ParseMultipleMessages();

      //var settings = new FixParserSettings();
      //settings.RegisterMessageTypes<Order>();

      //var parser = new FixParser(settings);
      //parser.Parse<Order>("8=FIX.4.2|9=19|35=D|55=AAPL|54=1|10=186|1=account1|44=12.5|");

      string sampleInput = "35=A|55=TestTag55|62=35|67=56.123|";
      var message = new MessageBuilder().AddRaw(sampleInput).Build();
      var propertyMapper = new TagToPropertyMapper(new SubPropertySetterFactory());
      propertyMapper.Map<TestTypeParent>();

      var parser = new MessageParser(propertyMapper, new CompositePropertySetter(), new ValidatorCollection(IntegerToFixConverter.Instance), new RapideFix.Business.Data.MessageParserOptions());
      parser.Parse<TestTypeParent>(message);
      parser.Parse<TestTypeParent>(message);
      parser.Parse<TestTypeParent>(message);
      parser.Parse<TestTypeParent>(message);

      Console.WriteLine("Wait");
      Thread.Sleep(3000);
      Console.WriteLine("Start");

      for(int i = 0; i < 1000000; i++)
      {
        parser.Parse<TestTypeParent>(message);
        parser.Parse<TestTypeParent>(message);
        parser.Parse<TestTypeParent>(message);
      }

    }
  }

}
