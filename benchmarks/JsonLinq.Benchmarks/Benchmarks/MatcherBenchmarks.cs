using System.Text.Json.Nodes;
using BenchmarkDotNet.Attributes;
using JsonLinq;

namespace JsonLinq.Benchmarks.Benchmarks;

[Config(typeof(JsonLinqBenchmarkConfig))]
public class MatcherBenchmarks
{
    private JsonNode? _testNode = null!;

    [GlobalSetup]
    public void Setup()
    {
        _testNode = JsonNode.Parse("""
        {
            "name": "Alice",
            "age": 32,
            "salary": 95000,
            "active": true,
            "department": "Engineering"
        }
        """);
    }

    [Benchmark]
    public bool EqualityPredicate()
    {
        return _testNode.Value<string>("name") == "Alice";
    }

    [Benchmark]
    public bool InequalityPredicate()
    {
        return _testNode.Value<string>("name") != "Bob";
    }

    [Benchmark]
    public bool GreaterThanPredicate()
    {
        return _testNode.Value<int>("age") > 25;
    }

    [Benchmark]
    public bool LessThanPredicate()
    {
        return _testNode.Value<decimal>("salary") < 100000;
    }

    [Benchmark]
    public bool ContainsPredicate()
    {
        return _testNode.Value<string>("name")?.Contains("Ali") ?? false;
    }
}
