# Plan: Remove Abstractions

## TL;DR
Remove dead abstractions (`IJsonQueryable`, `ICondition`, `IQueryResult<T>`, `JsonCondition`, `IMatcher`, `Matcher`, `ConditionBuilder`).

---

## Phase 1 — Delete Dead Abstraction Files

1. Delete `src/JsonLinq/Abstractions/IJsonQueryable.cs`
2. Delete `src/JsonLinq/Abstractions/ICondition.cs`
3. Delete `src/JsonLinq/Abstractions/IQueryResult.cs`
4. Delete `src/JsonLinq/Core/JsonCondition.cs`
5. Delete `src/JsonLinq/Core/IMatcher.cs`
6. Delete `src/JsonLinq/Core/Matcher.cs`
7. Delete `src/JsonLinq/Builders/ConditionBuilder.cs`

After step 7, the `Abstractions/` folder is empty and can be removed.

---

## Phase 2 — Update Core: QueryEngine + JsonQuery

**Parallel with each other:**

8. `src/JsonLinq/Core/QueryEngine.cs` — Remove `Filter(IEnumerable<JsonNode?>, JsonCondition)` method and the `IMatcher` constructor parameter. Keep: `Sort`, `GroupBy`, `Sum`, `Avg`, `Max`, `Min`.
   - Constructor changes from `QueryEngine(IMatcher matcher)` → `QueryEngine()` (no-arg).
   - `_engine` in `JsonQuery` still needed for aggregations/sorting.

9. `src/JsonLinq/Core/JsonQuery.cs` —
   - Remove `: IJsonQueryable` from class declaration.
   - Update `QueryEngine` construction call (remove `new Matcher()` arg).

---

## Phase 3 — Update LinqExtensions

10. `src/JsonLinq/Extensions/LinqExtensions.cs` — Remove any references to deleted abstractions.

---

## Phase 4 — Update Call Sites

*All parallel:*

11. `tests/JsonLinq.Tests/Unit/QueryEngineTests.cs` — Remove test for `engine.Filter()` (method deleted). If file tests other engine methods, keep those.

---

## Relevant Files

- `src/JsonLinq/Abstractions/IJsonQueryable.cs` — Delete
- `src/JsonLinq/Abstractions/ICondition.cs` — Delete
- `src/JsonLinq/Abstractions/IQueryResult.cs` — Delete (unused)
- `src/JsonLinq/Core/JsonCondition.cs` — Delete
- `src/JsonLinq/Core/IMatcher.cs` — Delete
- `src/JsonLinq/Core/Matcher.cs` — Delete
- `src/JsonLinq/Builders/ConditionBuilder.cs` — Delete
- `src/JsonLinq/Core/QueryEngine.cs` — Remove `Filter` method + `IMatcher` ctor param
- `src/JsonLinq/Core/JsonQuery.cs` — Remove interface, update `QueryEngine` construction
- `src/JsonLinq/Extensions/LinqExtensions.cs` — Remove references to deleted abstractions
- `tests/JsonLinq.Tests/Unit/QueryEngineTests.cs` — Remove Filter test

---

## Verification

1. `dotnet build` — No compile errors across all projects
2. `dotnet test` — All tests pass
3. Confirm no remaining references to `IJsonQueryable`, `ICondition`, `JsonCondition`, `Matcher`, `ConditionBuilder`

---

## Decisions

- `IQueryResult<T>` deleted — completely unused in codebase
- `QueryEngine` retained (not deleted) — still owns Sort, GroupBy, aggregation logic
- `LinqExtensions.Filter()` updated (not deleted) — it's a public LINQ-style alias, worth keeping
- Benchmarks updated — they must compile; perf characteristics slightly change (lambda vs. string dispatch) but that's acceptable
- Numeric lambdas: use `GetValue<decimal>()` for consistent type matching with existing aggregation behavior
