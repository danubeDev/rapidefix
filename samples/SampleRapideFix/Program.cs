﻿using RapideFix;
using SampleRapideFix.Data;

namespace SampleRapideFix
{
  class Program
  {
    static void Main(string[] args)
    {
      var settings = new FixParserSettings();
      settings.RegisterMessageTypes<Order>();

      var parser = new FixParser(settings);
      parser.Parse<Order>("8=FIX.4.2|9=19|35=D|55=AAPL|54=12|10=186|");
    }
  }
}
