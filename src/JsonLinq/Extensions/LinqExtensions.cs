using JsonLinq.Core;

namespace JsonLinq.Extensions;

/// <summary>
/// Convenience aliases that align with LINQ-like naming.
/// </summary>
public static class LinqExtensions
{
    /// <summary>
    /// Alias to <see cref="JsonQuery.Where"/>.
    /// </summary>
    public static JsonQuery Filter(this JsonQuery query, string field, string op, object? value) => query.Where(field, op, value);
}
