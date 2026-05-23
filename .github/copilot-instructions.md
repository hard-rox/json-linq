# Copilot Instructions

## Project Knowledge

For detailed project context — architecture, build commands, implementation patterns, and test conventions — load the [json-linq skill](.github/skills/json-linq/SKILL.md) before starting any implementation work.

## Naming Conventions

All public method names in `JsonLinq` must follow **C# LINQ naming exactly**.

Use the canonical LINQ names:

| Correct | Incorrect |
|---------|-----------|
| `FirstOrDefault` | `First` |
| `LastOrDefault` | `Last` |
| `SingleOrDefault` | `Single` |
| `Average` | `Avg` |
| `OrderBy` | `SortBy` |
| `OrderByDescending` | `SortByDesc` |
| `ThenBy` | — |
| `ThenByDescending` | — |
| `SelectMany` | — |
| `OfType` | — |
| `Reverse` | — |

When adding new query methods, check the [LINQ documentation](https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/linq/) and match the method name and semantics as closely as the JSON domain allows.
