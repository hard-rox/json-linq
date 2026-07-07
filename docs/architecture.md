# Architecture

JsonLinq uses an immutable fluent query pipeline.

## Main Components

- `JsonQuery`: public fluent API and immutable query state
- `JsonNodeExtensions`: typed helper accessors (`Value<T>`, `ValueAt<T>`) for predicate-based filtering
- `PathResolver`: dot-path traversal over `JsonNode`
- `JsonParser`: parse from string and file
- `JsonQueryException` / `InvalidPathException`: library-specific exception types

## Design Notes

- LINQ-like developer experience with clear method chaining
- Immutable query instances to avoid hidden state mutation
- Predicate-based filtering (`Where(Func<JsonNode?, bool>)`) replaces operator-string matching
- Sorting, grouping, and numeric aggregations are implemented in private static helpers inside `JsonQuery`
