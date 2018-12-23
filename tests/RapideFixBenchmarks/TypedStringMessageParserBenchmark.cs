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
  public class TypedStringMessageParserBenchmark
  {
    private IMessageParser<TestTypeParent, char> _parser;

    [GlobalSetup]
    public void Setup()
    {
      var propertyMapper = new TagToPropertyMapper();
      propertyMapper.Map<TestTypeParent>();
      var compositeSetter = new CompositePropertySetter(new SubPropertySetterFactory());

      _parser = new TypedStringMessageParser<TestTypeParent>(propertyMapper, compositeSetter, new ValidatorCollection(IntegerToFixConverter.Instance), new RapideFix.Business.Data.MessageParserOptions());
    }

    [Params(SampleFixMessagesSource.Sample0, SampleFixMessagesSource.Sample1)]
    public string Message { get; set; }

    [Benchmark]
    public void ParseString() => _parser.Parse(Message);
  }
}
