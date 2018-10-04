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
  public class TypedMessageParserBenchmark
  {
    private byte[] _message;
    private IMessageParser<TestTypeStruct> _parser;

    [GlobalSetup]
    public void Setup()
    {
      _message = new TestFixMessageBuilder("35=A|101=345|102=128.79|").Build();
      var propertyMapper = new TagToPropertyMapper();
      propertyMapper.Map<TestTypeStruct>();
      var compositeSetter = new CompositePropertySetter(new SubPropertySetterFactory());

      _parser = new TypedMessageParser<TestTypeStruct>(propertyMapper, compositeSetter, new ValidatorCollection(IntegerToFixConverter.Instance), new RapideFix.Business.Data.MessageParserOptions());
    }

    [Benchmark]
    public void ParseStruct() => _parser.Parse(_message);
  }
}
