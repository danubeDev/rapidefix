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
      var propertyMapper = new TagToPropertyMapper();
      propertyMapper.Map<TestTypeParent>();
      var compositeSetter = new CompositePropertySetter(new SubPropertySetterFactory());

      _parser = new MessageParser(propertyMapper, compositeSetter, new ValidatorCollection(IntegerToFixConverter.Instance), new RapideFix.Business.Data.MessageParserOptions());
    }

    [Benchmark]
    public void Parse() => _parser.Parse(_message);
  }
}
