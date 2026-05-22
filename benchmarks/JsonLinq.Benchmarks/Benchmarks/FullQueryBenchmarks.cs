using BenchmarkDotNet.Attributes;
using JsonLinq.Core;

namespace JsonLinq.Benchmarks.Benchmarks;

[Config(typeof(JsonLinqBenchmarkConfig))]
public class FullQueryBenchmarks
{
    [GlobalSetup]
    public void Setup() { /* data inlined as constants */ }

    [Benchmark]
    public int ComplexQueryChainSmall() =>
        JsonQuery.Parse(BenchmarkData.SmallJson)
            .From("employees")
            .Where("department", "==", "Engineering")
            .OrderByDescending("salary")
            .Count();

    [Benchmark]
    public int FilterAndAggregateSmall() =>
        JsonQuery.Parse(BenchmarkData.SmallJson)
            .From("employees")
            .Where("active", "==", true)
            .Count();

    [Benchmark]
    public int FilterAndGroupSmall() =>
        JsonQuery.Parse(BenchmarkData.SmallJson)
            .From("employees")
            .Where("age", ">", 26)
            .GroupBy("department")
            .Count();

    [Benchmark]
    public int ComplexQueryChainMedium() =>
        JsonQuery.Parse(BenchmarkData.MediumJson)
            .From("employees")
            .Where("department", "==", "Engineering")
            .Where("salary", ">", 900)
            .OrderBy("age")
            .Count();

    [Benchmark]
    public int MultiOrWhereSmall() =>
        JsonQuery.Parse(BenchmarkData.SmallJson)
            .From("employees")
            .Where("department", "==", "Engineering")
            .OrWhere("department", "==", "Sales")
            .OrWhere("department", "==", "Marketing")
            .Count();
}
