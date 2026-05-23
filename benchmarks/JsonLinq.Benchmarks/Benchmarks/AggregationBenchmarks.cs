using BenchmarkDotNet.Attributes;
using JsonLinq;

namespace JsonLinq.Benchmarks.Benchmarks;

[Config(typeof(JsonLinqBenchmarkConfig))]
public class AggregationBenchmarks
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
    public decimal SumSmall() => _querySmall.Sum("salary");

    [Benchmark]
    public decimal SumMedium() => _queryMedium.Sum("salary");

    [Benchmark]
    public decimal AvgSmall() => _querySmall.Average("salary");

    [Benchmark]
    public decimal MinSmall() => _querySmall.Min("salary");

    [Benchmark]
    public decimal MaxSmall() => _querySmall.Max("salary");

    [Benchmark]
    public int CountSmall() => _querySmall.Count();
}
