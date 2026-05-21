# JsonLinq Benchmarks

This directory contains BenchmarkDotNet-based performance benchmarks for JsonLinq core operations.

## Running Benchmarks

### Quick Run (Development)

```bash
dotnet run -p benchmarks/JsonLinq.Benchmarks/ -c Release
```

This uses `ShortRun` configuration (3 warmups, 5 measurements) for quick feedback.

### Full Run (CI)

```bash
dotnet run -p benchmarks/JsonLinq.Benchmarks/ -c Release -- --job Release
```

This runs with more comprehensive measurements.

## Results

Benchmark results are generated in:
- `BenchmarkDotNet.Artifacts/results/` — JSON, CSV, Markdown reports
- `BenchmarkDotNet.Artifacts/results/index.html` — Interactive HTML report

## Interpreting Results

Each benchmark reports:

- **Mean** — Average execution time per operation
- **StdDev** — Standard deviation (lower is more consistent)
- **Allocated** — Total memory allocated (in bytes)
- **Gen0/Gen1/Gen2** — GC collections during benchmark

## Benchmarks Included

### Filter Operations (`FilterBenchmarks.cs`)
- Single `Where` clause on small/medium datasets
- Multiple chained `Where` clauses
- `OrWhere` combinations
- Numeric comparisons (`>`, `<`, etc.)

### Aggregations (`AggregationBenchmarks.cs`)
- `Sum()`, `Avg()`, `Min()`, `Max()` on numeric fields
- `Count()` on result sets

### Transformations (`TransformationBenchmarks.cs`)
- `SortBy()` in ascending/descending order
- `GroupBy()` with various group sizes
- `Chunk()` pagination
- `Distinct()` deduplication

### Path Resolution (`PathResolverBenchmarks.cs`)
- Simple path: `"users"`
- Nested path: `"users.0.name"`
- Array indexing: `"users.5"`
- Invalid/missing paths

### Condition Matching (`MatcherBenchmarks.cs`)
- Equality (`==`, `!=`)
- Relational (`>`, `<`, `>=`, `<=`)
- String operations (`contains`)

### End-to-End Queries (`FullQueryBenchmarks.cs`)
- Complex query chains: Filter → Sort → GroupBy
- Real-world scenarios combining multiple operations

## Performance Expectations

| Operation | Dataset | Expected Throughput |
|-----------|---------|---------------------|
| Simple Where | 10 items | >100k ops/sec |
| GroupBy | 10 items | >50k ops/sec |
| Sort | 10 items | >50k ops/sec |
| Aggregation | 10 items | >100k ops/sec |

*(These are baseline expectations; actual results vary by hardware and .NET version.)*

## Regression Detection

Baseline results are stored in `.benchmarks/baseline.json`. 

To detect regressions:

```bash
dotnet run -p benchmarks/JsonLinq.Benchmarks/ -c Release --artifacts baseline.json
```

A >10% regression in Mean time should be investigated.

## Adding New Benchmarks

1. Create a new `[Benchmark]` method in the appropriate class or a new file
2. Use `[GlobalSetup]` to initialize data once per benchmark run
3. Follow naming: `OperationNameVariant` (e.g., `WhereSmallDataset`)
4. Ensure the operation is representative of real usage

Example:

```csharp
[Benchmark]
public int MyNewBenchmark()
{
    return JsonQuery.Parse(_json)
        .From("users")
        .Where("field", "op", value)
        .Count();
}
```

## CI Integration

See `.github/workflows/benchmark.yml` for automated benchmark runs on schedule and pull requests.
