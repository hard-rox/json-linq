# JsonLinq

JsonLinq is a .NET class library for fluent, LINQ-like querying over JSON data.

## Features

- Fluent query pipeline for JSON arrays and objects
- Dot-path navigation with `From`, `Find`, and `At`
- Filtering with `Where` and `OrWhere`
- Aggregations: `Sum`, `Avg`, `Min`, `Max`, `Count`
- Transformations: `Sort`, `SortBy`, `GroupBy`, `Chunk`, `Distinct`
- Example console app and test suite

## Installation

```bash
dotnet add package JsonLinq
```

## Quick Start

```csharp
using JsonLinq.Core;

var json = """
{
  "users": [
    { "name": "Ava", "age": 30 },
    { "name": "Ben", "age": 25 }
  ]
}
""";

var users = JsonQuery.Parse(json)
    .From("users")
    .Where("age", ">", 26)
    .Get();
```

See [docs/getting-started.md](docs/getting-started.md) and [docs/api-reference.md](docs/api-reference.md).
