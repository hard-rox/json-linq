using BenchmarkDotNet.Attributes;
using JsonLinq.Core;
using JsonLinq.Extensions;

namespace JsonLinq.Benchmarks.Benchmarks;

[Config(typeof(JsonLinqBenchmarkConfig))]
public class FilterBenchmarks
{
    private JsonQuery _querySmall = null!;
    private JsonQuery _queryMedium = null!;

    [GlobalSetup]
    public void Setup()
    {
        _querySmall = JsonQuery.Parse(BenchmarkData.SmallJson).From("employees");
        _queryMedium = JsonQuery.Parse(BenchmarkData.MediumJson).From("employees");
    }

    [Benchmark]
    public int SimpleWhereSmall() =>
        _querySmall.Where(n => n.Value<string>("department") == "Engineering").Count();

    [Benchmark]
    public int SimpleWhereMedium() =>
        _queryMedium.Where(n => n.Value<string>("department") == "Engineering").Count();

    [Benchmark]
    public int MultipleWheresSmall() =>
        _querySmall
            .Where(n => n.Value<string>("department") == "Engineering")
            .Where(n => n.Value<bool>("active") == true)
            .Count();

    [Benchmark]
    public int OrWhereSmall() =>
        _querySmall
            .Where(n => n.Value<string>("department") == "Engineering" || n.Value<string>("department") == "Sales")
            .Count();

    [Benchmark]
    public int NumericComparisonSmall() =>
        _querySmall.Where(n => n.Value<decimal>("salary") > 900).Count();
}
