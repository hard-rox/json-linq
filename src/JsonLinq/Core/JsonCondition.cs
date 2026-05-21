using JsonLinq.Abstractions;

namespace JsonLinq.Core;

/// <summary>
/// Default condition model used by the query engine.
/// </summary>
public sealed record JsonCondition(string Field, string Operator, object? Value) : ICondition;
