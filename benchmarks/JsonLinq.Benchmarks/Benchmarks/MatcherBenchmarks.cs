using System.Text.Json.Nodes;
using BenchmarkDotNet.Attributes;
using JsonLinq.Core;

namespace JsonLinq.Benchmarks.Benchmarks;

[Config(typeof(JsonLinqBenchmarkConfig))]
public class MatcherBenchmarks
{
    private Matcher _matcher = null!;
    private JsonNode? _testNode = null!;

    [GlobalSetup]
    public void Setup()
    {
        _matcher = new Matcher();
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
    public bool EqualityOperator()
    {
        return _matcher.IsMatch(_testNode, new JsonCondition("name", "==", "Alice"));
    }

    [Benchmark]
    public bool InequalityOperator()
    {
        return _matcher.IsMatch(_testNode, new JsonCondition("name", "!=", "Bob"));
    }

    [Benchmark]
    public bool GreaterThanOperator()
    {
        return _matcher.IsMatch(_testNode, new JsonCondition("age", ">", 25));
    }

    [Benchmark]
    public bool LessThanOperator()
    {
        return _matcher.IsMatch(_testNode, new JsonCondition("salary", "<", 100000));
    }

    [Benchmark]
    public bool ContainsOperator()
    {
        return _matcher.IsMatch(_testNode, new JsonCondition("name", "contains", "Ali"));
    }
}
