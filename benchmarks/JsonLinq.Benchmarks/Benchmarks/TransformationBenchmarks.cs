using BenchmarkDotNet.Attributes;
using JsonLinq;

namespace JsonLinq.Benchmarks.Benchmarks;

[Config(typeof(JsonLinqBenchmarkConfig))]
public class TransformationBenchmarks
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
    public int OrderByAscSmall() => _querySmall.OrderBy("age").Count();

    [Benchmark]
    public int OrderByDescSmall() => _querySmall.OrderByDescending("age").Count();

    [Benchmark]
    public int GroupBySmall() => _querySmall.GroupBy("department").Count();

    [Benchmark]
    public int GroupByMedium() => _queryMedium.GroupBy("department").Count();

    [Benchmark]
    public int ChunkSmall() => _querySmall.Chunk(3).Count();

    [Benchmark]
    public int DistinctSmall() => _querySmall.Distinct().Count();
}
