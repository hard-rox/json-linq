# API Reference

## Entry points

- `JsonQuery.Parse(string json)`
- `JsonQuery.ParseFile(string path)`
- `JsonLinq.JsonLinq.Parse(string json)`
- `JsonLinq.JsonLinq.ParseFile(string path)`

## Navigation

- `From(path)`
- `Find(path)`
- `At(path)`

## Filtering

- `Where(field, op, value)`
- `OrWhere(field, op, value)`

## Transformations

- `Sort(order)`
- `SortBy(field, order)`
- `GroupBy(field)`
- `Chunk(size)`
- `Distinct()`

## Aggregations

- `Count()`
- `Sum(field)`
- `Avg(field)`
- `Min(field)`
- `Max(field)`

## Accessors

- `First()`
- `Last()`
- `Nth(index)`
- `Exists()`
- `Get()`
