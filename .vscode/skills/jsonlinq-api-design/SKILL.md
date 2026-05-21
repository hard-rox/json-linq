---
name: jsonlinq-api-design
description: Keep JsonLinq API fluent, immutable, and consistent with .NET LINQ ergonomics.
---

# JsonLinq API Design Skill

## Rules

- Keep query operations immutable; return a new query instance.
- Keep method naming consistent with C# conventions.
- Maintain operator handling parity for `==`, `!=`, `>`, `>=`, `<`, `<=`, `contains`, `in`.
- Prefer small helper types over monolithic methods.
- Add/maintain XML docs for all public members.

## Testing

- Add tests for every new API behavior.
- Use xunit assertions and NSubstitute where mocking is needed.
