namespace JsonLinq.Abstractions;

/// <summary>
/// Represents a typed query result wrapper.
/// </summary>
/// <typeparam name="T">The result item type.</typeparam>
public interface IQueryResult<out T>
{
    /// <summary>
    /// Gets the result items.
    /// </summary>
    IReadOnlyList<T> Items { get; }
}
