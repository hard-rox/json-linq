---
name: json-linq
description: 'JsonLinq project knowledge. Use when adding query methods, writing tests, implementing features, or understanding architecture for this C# JSON querying library.'
user-invocable: false
---

# JsonLinq Project

## Project Structure

```
src/JsonLinq/
  Core/
    JsonQuery.cs          # Single fluent API class — all query methods live here
  Extensions/
    JsonNodeExtensions.cs # Value<T>(key) and ValueAt<T>(path) extension methods on JsonNode?
    LinqExtensions.cs     # (currently empty shell)
  Utilities/
    PathResolver.cs       # Dot-notated path resolution: "employees.0.address.city"
    JsonValueHelper.cs    # ToComparable() — used for sort and group-by
    JsonParser.cs         # Wraps System.Text.Json parsing + file I/O
  Exceptions/
    JsonQueryException.cs
    InvalidPathException.cs
  JsonLinq.cs             # Public static factory: Parse(), ParseFile(), ParseFileAsync()
  GlobalUsings.cs         # global using System.Collections.ObjectModel, System.Text.Json, System.Text.Json.Nodes

tests/JsonLinq.Tests/
  Unit/
    JsonQueryTests.cs         # Main test file for JsonQuery methods
    JsonNodeExtensionsTests.cs
    PathResolverTests.cs
    JsonParserTests.cs
    QueryBuilderTests.cs
  Integration/
    EndToEndTests.cs
  Fixtures/
    SampleDataFixture.cs  # 3-employee JSON dataset (Alice/Bob/Carol)
    JsonTestHelper.cs     # GetString(), GetDecimal() helpers

benchmarks/JsonLinq.Benchmarks/
  Benchmarks/
    FilterBenchmarks.cs
    FullQueryBenchmarks.cs
    AggregationBenchmarks.cs
    ...
```

## Build & Test Commands

```bash
dotnet build
dotnet test
dotnet test --filter "FullyQualifiedName~JsonQueryTests"
```

## Architecture

`JsonQuery` is the **only** public query class. It is immutable — every method returns a new instance.

### Constructor (private)
```csharp
private JsonQuery(JsonNode root, IReadOnlyList<JsonNode?> scope, IReadOnlyList<JsonNode?> result)
```
- `root` — the original parsed document, never changes
- `scope` — the active collection set by `From()`, used for union-style operations
- `result` — the current working set, narrowed by `Where`, `Take`, `Skip`, etc.

### Adding a new query method
1. Return a new `JsonQuery` — never mutate `_result`
2. Use `_result.Something().ToList().AsReadOnly()` for the new result
3. Pass `_root` and `_scope` unchanged unless the method resets scope (like `From` and `GroupBy` do)
4. Add private static helpers at the bottom of the class if needed
5. Follow C# LINQ naming exactly (see `.github/copilot-instructions.md`)

```csharp
public JsonQuery Reverse()
{
    IReadOnlyList<JsonNode?> reversed = _result.Reverse().ToList().AsReadOnly();
    return new JsonQuery(_root, _scope, reversed);
}
```

### Path resolution
Use `PathResolver.Resolve(node, "dot.notated.path")` — handles arrays by numeric index (`employees.0.name`).

### Predicate helpers
`JsonNodeExtensions` provides concise access inside `Where` lambdas:
```csharp
.Where(n => n.Value<string>("department") == "Engineering")
.Where(n => n.ValueAt<string>("address.city") == "Oakland")
.Where(n => n.Value<int>("age") > 30)
```

### Aggregation / sort internals
Private static methods at the bottom of `JsonQuery`:
- `Sort(source, path, descending)` — uses `JsonValueHelper.ToComparable`
- `GroupByField(source, path)` — returns `IReadOnlyDictionary<string, IReadOnlyList<JsonNode?>>`
- `SelectNumbers(source, path)` — yields `decimal`, throws `JsonQueryException` on non-numeric

## Test Conventions

- Test class: `public sealed class MyTests`
- Fixture: `private readonly SampleDataFixture _fixture = new();` gives `_fixture.Json` (3 employees)
- Assertion helpers: `JsonTestHelper.GetString(node, "name")`, `JsonTestHelper.GetDecimal(node, "salary")`
- New `JsonQuery` tests go in `JsonQueryTests.cs` under the appropriate `// ── Section ──` comment block
- New extension method tests go in `JsonNodeExtensionsTests.cs`
