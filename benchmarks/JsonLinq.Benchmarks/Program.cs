using BenchmarkDotNet.Running;
using JsonLinq.Benchmarks;

var config = new JsonLinqBenchmarkConfig();
var summary = BenchmarkRunner.Run(typeof(Program).Assembly, config);


