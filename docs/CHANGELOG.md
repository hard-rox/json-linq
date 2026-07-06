# Changelog

## 1.0.0

### Highlights

- Stable fluent API over `System.Text.Json.Nodes` with immutable query chaining.
- Multi-target support for `net6.0`, `net8.0`, `net9.0`, and `net10.0`.
- Public API naming aligned with C# LINQ conventions (`Average`, `FirstOrDefault`, `LastOrDefault`, `SingleOrDefault`, `OrderBy`).
- Package provenance improvements with SourceLink and repository metadata.

### API Notes

- Filtering is predicate-based: `Where(Func<JsonNode?, bool>)`.
- There is no `OrWhere`; combine predicates with `||` or merge result sets manually.
- Preferred value access inside predicates uses extension helpers:
	- `Value<T>(key)`
	- `ValueAt<T>(path)`

### Tooling and CI

- CI validates test/coverage runs per target framework.
- Publish workflow packs and publishes all configured target assets from a single package.

### Package Links

- Repository: https://github.com/hard-rox/json-linq
- NuGet: https://www.nuget.org/packages/JsonLinq
