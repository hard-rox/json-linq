# Architecture

JsonLinq uses an immutable fluent query pipeline.

## Main Components

- `JsonQuery`: public fluent API and immutable query state
- `QueryEngine`: execution of filter/sort/group/aggregate operations
- `Matcher`: condition matching (`==`, `!=`, `>`, `<`, `contains`, `in`)
- `PathResolver`: dot-path traversal over `JsonNode`
- `JsonParser`: parse from string and file

## Design Notes

- LINQ-like developer experience with clear method chaining
- Immutable query instances to avoid hidden state mutation
- Small composable services for testability
