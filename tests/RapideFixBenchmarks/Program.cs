using BenchmarkDotNet.Running;

namespace RapideFixBenchmarks
{
  public class Program
  {
    public static void Main(string[] args)
    {
      var summaryTyped = BenchmarkRunner.Run<TypedMessageParserBenchmark>();
      var summary = BenchmarkRunner.Run<MessageParserBenchmark>();
    }
  }
}
