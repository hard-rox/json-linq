using BenchmarkDotNet.Attributes;
using JsonLinq.Core;

namespace JsonLinq.Benchmarks.Benchmarks;

[Config(typeof(JsonLinqBenchmarkConfig))]
public class TransformationBenchmarks
{
    private JsonQuery _querySmall = null!;
    private JsonQuery _queryMedium = null!;

    [GlobalSetup]
    public void Setup()
    {
        _querySmall = JsonQuery.Parse(BenchmarkData.SmallJson).From("users");
        _queryMedium = JsonQuery.Parse(BenchmarkData.MediumJson).From("users");
    }

    [Benchmark]
    public int SortByAscSmall() => _querySmall.SortBy("age", "asc").Count();

    [Benchmark]
    public int SortByDescSmall() => _querySmall.SortBy("age", "desc").Count();

    [Benchmark]
    public int GroupBySmall() => _querySmall.GroupBy("department").Count();

    [Benchmark]
    public int GroupByMedium() => _queryMedium.GroupBy("department").Count();

    [Benchmark]
    public int ChunkSmall() => _querySmall.Chunk(3).Count();

    [Benchmark]
    public int DistinctSmall() => _querySmall.Distinct().Count();
}
