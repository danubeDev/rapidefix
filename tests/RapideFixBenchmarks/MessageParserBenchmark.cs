using System.Linq;
using BenchmarkDotNet.Attributes;
using RapideFix;
using RapideFix.Business;
using RapideFix.Parsers;
using RapideFix.Validation;
using RapideFixFixture;
using RapideFixFixture.TestTypes;

namespace RapideFixBenchmarks
{
  [CoreJob]
  [MarkdownExporter, HtmlExporter]
  [MemoryDiagnoser]
  public class MessageParserBenchmark
  {
    private byte[] _message;
    private IMessageParser<object, byte> _parser;

    [GlobalSetup]
    public void Setup()
    {
      string sampleInput = SampleFixMessagesSource.GetTestTypeParentMessageBodies().First().First() as string;
      _message = new TestFixMessageBuilder(sampleInput).Build();
      var propertyMapper = new TagToPropertyMapper(new SubPropertySetterFactory());
      propertyMapper.Map<TestTypeParent>();

      _parser = new MessageParser(propertyMapper, new CompositePropertySetter(), new ValidatorCollection(IntegerToFixConverter.Instance), new RapideFix.Business.Data.MessageParserOptions());
    }

    [Benchmark]
    public void Parse() => _parser.Parse(_message);
  }
}
