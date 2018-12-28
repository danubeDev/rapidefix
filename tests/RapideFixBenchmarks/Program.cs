using System;
using System.IO;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;
using RapideFixFixture.TestTypes;

namespace RapideFixBenchmarks
{
  public class Program
  {
    private const string DocumentationPath = @"..\..\..\..\..\samples\Documentation.DocFx\benchmarks";

    public static void Main(string[] args)
    {
      IConfig customConfig = DefaultConfig.Instance;
      if(Directory.Exists(DocumentationPath))
      {
        customConfig = customConfig.WithArtifactsPath(DocumentationPath);
      }

      //var summaryTyped = BenchmarkRunner.Run<TypedMessageParserBenchmark>(customConfig);
      //var summary = BenchmarkRunner.Run<MessageParserBenchmark>(customConfig);
      var summaryString = BenchmarkRunner.Run<TypedStringMessageParserBenchmark>(customConfig);
      TestTypeSize();
    }

    public static void TestTypeSize()
    {
      GC.Collect();
      var sizeBase = GC.GetAllocatedBytesForCurrentThread();
      var testType = new TestTypeParent();
      var newSize = GC.GetAllocatedBytesForCurrentThread();
      Console.WriteLine($"{testType.GetType().Name}: {newSize - sizeBase}");
    }
  }
}
