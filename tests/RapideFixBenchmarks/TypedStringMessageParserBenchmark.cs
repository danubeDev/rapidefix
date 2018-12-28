using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
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

    [ParamsSource(nameof(Params))]
    public DisplayParam Message { get; set; }

    public IEnumerable<DisplayParam> Params => new[] { new DisplayParam(SampleFixMessagesSource.Sample0), new DisplayParam(SampleFixMessagesSource.Sample1) };

    [Benchmark]
    public void ParseString() => _parser.Parse(Message.Value);
  }
}
