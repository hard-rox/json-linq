# API Reference

The primary public API is `JsonQuery` in the `JsonLinq` namespace.

## Entry points

- `JsonQuery.Parse(string json)`
- `JsonQuery.ParseFile(string filePath)`
- `JsonQuery.ParseFileAsync(string filePath, CancellationToken cancellationToken = default)`

## Navigation

- `JsonQuery From(string path)`
- `JsonNode? Find(string path)`
- `JsonNode? At(string path)`

## Filtering

- `JsonQuery Where(Func<JsonNode?, bool> predicate)`

Use predicate-based filtering with extension helpers:

- `T? Value<T>(this JsonNode? node, string key)`
- `T? ValueAt<T>(this JsonNode? node, string path)`

There is no `OrWhere`. Use one predicate with `||` or combine results manually.

## Query transformations

- `JsonQuery OrderBy(string field)`
- `JsonQuery OrderByDescending(string field)`
- `JsonQuery GroupBy(string field)`
- `JsonQuery Chunk(int size)`
- `JsonQuery Select(params string[] fields)`
- `JsonQuery Distinct()`
- `JsonQuery Take(int count)`
- `JsonQuery Skip(int count)`
- `JsonQuery Copy()`

## Aggregations

- `int Count()`
- `decimal Sum(string field)`
- `decimal Average(string field)`
- `decimal Min(string field)`
- `decimal Max(string field)`

## Element access and terminal operations

- `JsonNode? FirstOrDefault()`
- `JsonNode? LastOrDefault()`
- `JsonNode? SingleOrDefault()`
- `JsonNode? Nth(int index)`
- `bool Exists()`
- `IReadOnlyList<JsonNode?> ToList()`

## Exceptions

- `JsonLinq.Exceptions.JsonQueryException`
- `JsonLinq.Exceptions.InvalidPathException`

## Utilities (public)

- `JsonLinq.Utilities.PathResolver.Resolve(JsonNode? root, string path)`
- `JsonLinq.Utilities.JsonParser.Parse(string json)`
- `JsonLinq.Utilities.JsonParser.ParseFile(string path)`
- `JsonLinq.Utilities.JsonParser.ParseFileAsync(string path, CancellationToken cancellationToken = default)`
