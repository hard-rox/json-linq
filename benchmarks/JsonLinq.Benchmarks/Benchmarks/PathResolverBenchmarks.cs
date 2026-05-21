using System.Text.Json.Nodes;
using BenchmarkDotNet.Attributes;
using JsonLinq.Utilities;

namespace JsonLinq.Benchmarks.Benchmarks;

[Config(typeof(JsonLinqBenchmarkConfig))]
public class PathResolverBenchmarks
{
    private JsonNode? _root;

    [GlobalSetup]
    public void Setup()
    {
        _root = JsonNode.Parse(BenchmarkData.SmallJson);
    }

    [Benchmark]
    public JsonNode? SimplePathResolution() => PathResolver.Resolve(_root, "users");

    [Benchmark]
    public JsonNode? NestedPathResolution() => PathResolver.Resolve(_root, "users.0.name");

    [Benchmark]
    public JsonNode? NumericIndexResolution() => PathResolver.Resolve(_root, "users.5");

    [Benchmark]
    public JsonNode? DeepPathResolution() => PathResolver.Resolve(_root, "users.3.department");

    [Benchmark]
    public JsonNode? MissingPathResolution() => PathResolver.Resolve(_root, "nonexistent");
}
