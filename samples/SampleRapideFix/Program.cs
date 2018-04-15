using RapideFix;
using System;

namespace SampleRapideFix
{
  class Program
  {
    static void Main(string[] args)
    {
      var lib = new Lib();
      Console.WriteLine(lib.Hello());
    }
  }
}
