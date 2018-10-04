﻿using System.Collections.Generic;
using System.Linq;
using RapideFix;
using RapideFix.Business;
using RapideFix.Parsers;
using RapideFix.Validation;
using RapideFixFixture;
using RapideFixFixture.TestTypes;
using SampleRapideFix.Data;

namespace SampleRapideFix
{
  class Program
  {
    static void Main(string[] args)
    {
      //var settings = new FixParserSettings();
      //settings.RegisterMessageTypes<Order>();

      //var parser = new FixParser(settings);
      //parser.Parse<Order>("8=FIX.4.2|9=19|35=D|55=AAPL|54=1|10=186|1=account1|44=12.5|");

      string sampleInput = "35=A|55=TestTag55|56=2|57=Enumerable1|57=Enumerable2|62=35|67=56.123|";
      var message = new TestFixMessageBuilder(sampleInput).Build();
      var propertyMapper = new TagToPropertyMapper();
      propertyMapper.Map<TestTypeParent>();
      var compositeSetter = new CompositePropertySetter(new SubPropertySetterFactory());

      var parser = new MessageParser(propertyMapper, compositeSetter, new ValidatorCollection(IntegerToFixConverter.Instance), new RapideFix.Business.Data.MessageParserOptions());
      parser.Parse<TestTypeParent>(message);
      parser.Parse<TestTypeParent>(message);
      parser.Parse<TestTypeParent>(message);
      parser.Parse<TestTypeParent>(message);
    }
  }

  public class T
  {
    public IEnumerable<int> A { get; set; }
  }
}
