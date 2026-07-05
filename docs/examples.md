# Examples

See the runnable console app under `examples/JsonLinq.Examples`.

Run it with:

```bash
dotnet run --project examples/JsonLinq.Examples
```

`Program.cs` showcases every public API from the [README](../README.md), grouped by category:

- Creation — `Parse`, `ParseFile`, `ParseFileAsync`
- Navigation — `From`, `Find`, `At`
- Filtering — `Where` (with the `Value<T>` and `ValueAt<T>` extensions)
- Ordering — `OrderBy`, `OrderByDescending`
- Grouping & chunking — `GroupBy`, `Chunk`
- Projection — `Select`, `Distinct`
- Pagination — `Take`, `Skip`
- Aggregation — `Sum`, `Average`, `Max`, `Min`, `Count`
- Element access — `FirstOrDefault`, `LastOrDefault`, `SingleOrDefault`, `Nth`, `Exists`
- Terminal & utility — `ToList`, `Copy`
