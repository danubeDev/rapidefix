
BenchmarkDotNet=v0.11.5, OS=Windows 10.0.17134.950 (1803/April2018Update/Redstone4)
Intel Xeon CPU E5-2673 v3 2.40GHz, 1 CPU, 2 logical and 2 physical cores
.NET Core SDK=3.0.100
  [Host] : .NET Core 3.0.0 (CoreCLR 4.700.19.46205, CoreFX 4.700.19.46214), 64bit RyuJIT
  Core   : .NET Core 3.0.0 (CoreCLR 4.700.19.46205, CoreFX 4.700.19.46214), 64bit RyuJIT

Job=Core  Runtime=Core  

      Method |  Message |     Mean |    Error |   StdDev |  Gen 0 | Gen 1 | Gen 2 | Allocated |
------------ |--------- |---------:|---------:|---------:|-------:|------:|------:|----------:|
 **ParseString** | **35= L:25** | **529.6 ns** | **5.994 ns** | **5.607 ns** | **0.0086** |     **-** |     **-** |     **136 B** |
 **ParseString** | **35= L:34** | **661.9 ns** | **7.497 ns** | **7.012 ns** | **0.0105** |     **-** |     **-** |     **176 B** |
