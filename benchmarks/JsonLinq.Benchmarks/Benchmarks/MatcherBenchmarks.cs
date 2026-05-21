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
            "name": "Ava",
            "age": 30,
            "salary": 1000,
            "active": true
        }
        """);
    }

    [Benchmark]
    public bool EqualityOperator()
    {
        return _matcher.IsMatch(_testNode, new JsonCondition("name", "==", "Ava"));
    }

    [Benchmark]
    public bool InequalityOperator()
    {
        return _matcher.IsMatch(_testNode, new JsonCondition("name", "!=", "Ben"));
    }

    [Benchmark]
    public bool GreaterThanOperator()
    {
        return _matcher.IsMatch(_testNode, new JsonCondition("age", ">", 25));
    }

    [Benchmark]
    public bool LessThanOperator()
    {
        return _matcher.IsMatch(_testNode, new JsonCondition("salary", "<", 1200));
    }

    [Benchmark]
    public bool ContainsOperator()
    {
        return _matcher.IsMatch(_testNode, new JsonCondition("name", "contains", "va"));
    }
}
