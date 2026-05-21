using JsonLinq.Core;

namespace JsonLinq.Builders;

/// <summary>
/// Builds reusable query conditions.
/// </summary>
public static class ConditionBuilder
{
    /// <summary>
    /// Creates a condition from field, operator, and value.
    /// </summary>
    public static JsonCondition Create(string field, string op, object? value) => new(field, op, value);
}
