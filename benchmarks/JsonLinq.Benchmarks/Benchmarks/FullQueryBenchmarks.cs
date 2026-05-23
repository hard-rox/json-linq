using BenchmarkDotNet.Attributes;
using JsonLinq;

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
            .Where(n => n.Value<string>("department") == "Engineering")
            .OrderByDescending("salary")
            .Count();

    [Benchmark]
    public int FilterAndAggregateSmall() =>
        JsonQuery.Parse(BenchmarkData.SmallJson)
            .From("employees")
            .Where(n => n.Value<bool>("active") == true)
            .Count();

    [Benchmark]
    public int FilterAndGroupSmall() =>
        JsonQuery.Parse(BenchmarkData.SmallJson)
            .From("employees")
            .Where(n => n.Value<int>("age") > 26)
            .GroupBy("department")
            .Count();

    [Benchmark]
    public int ComplexQueryChainMedium() =>
        JsonQuery.Parse(BenchmarkData.MediumJson)
            .From("employees")
            .Where(n => n.Value<string>("department") == "Engineering")
            .Where(n => n.Value<decimal>("salary") > 900)
            .OrderBy("age")
            .Count();

    [Benchmark]
    public int MultiOrWhereSmall() =>
        JsonQuery.Parse(BenchmarkData.SmallJson)
            .From("employees")
            .Where(n => n.Value<string>("department") == "Engineering"
                     || n.Value<string>("department") == "Sales"
                     || n.Value<string>("department") == "Marketing")
            .Count();
}
