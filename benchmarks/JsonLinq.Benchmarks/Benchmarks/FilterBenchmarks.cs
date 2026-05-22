using BenchmarkDotNet.Attributes;
using JsonLinq.Core;

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
        _querySmall.Where("department", "==", "Engineering").Count();

    [Benchmark]
    public int SimpleWhereMedium() =>
        _queryMedium.Where("department", "==", "Engineering").Count();

    [Benchmark]
    public int MultipleWheresSmall() =>
        _querySmall
            .Where("department", "==", "Engineering")
            .Where("active", "==", true)
            .Count();

    [Benchmark]
    public int OrWhereSmall() =>
        _querySmall
            .Where("department", "==", "Engineering")
            .OrWhere("department", "==", "Sales")
            .Count();

    [Benchmark]
    public int NumericComparisonSmall() =>
        _querySmall.Where("salary", ">", 900).Count();
}
