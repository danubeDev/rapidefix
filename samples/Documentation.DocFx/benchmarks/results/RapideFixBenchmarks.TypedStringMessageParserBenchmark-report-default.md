
BenchmarkDotNet=v0.11.3, OS=Windows 10.0.17134.523 (1803/April2018Update/Redstone4), VM=Hyper-V
Intel Xeon CPU E5-2673 v3 2.40GHz, 1 CPU, 2 logical and 2 physical cores
.NET Core SDK=2.2.101
  [Host] : .NET Core 2.2.0 (CoreCLR 4.6.27110.04, CoreFX 4.6.27110.04), 64bit RyuJIT
  Core   : .NET Core 2.2.0 (CoreCLR 4.6.27110.04, CoreFX 4.6.27110.04), 64bit RyuJIT

Job=Core  Runtime=Core  

      Method |  Message |     Mean |     Error |    StdDev | Gen 0/1k Op | Gen 1/1k Op | Gen 2/1k Op | Allocated Memory/Op |
------------ |--------- |---------:|----------:|----------:|------------:|------------:|------------:|--------------------:|
 **ParseString** | **35= L:25** | **1.043 us** | **0.0206 us** | **0.0406 us** |      **0.0172** |           **-** |           **-** |               **136 B** |
 **ParseString** | **35= L:34** | **1.261 us** | **0.0240 us** | **0.0295 us** |      **0.0248** |           **-** |           **-** |               **184 B** |