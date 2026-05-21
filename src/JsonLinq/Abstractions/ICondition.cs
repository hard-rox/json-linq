namespace JsonLinq.Abstractions;

/// <summary>
/// Represents a filter condition used by query operations.
/// </summary>
public interface ICondition
{
    /// <summary>
    /// Gets the field path used by the condition.
    /// </summary>
    string Field { get; }

    /// <summary>
    /// Gets the comparison operator.
    /// </summary>
    string Operator { get; }

    /// <summary>
    /// Gets the target value for comparison.
    /// </summary>
    object? Value { get; }
}
