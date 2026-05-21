using JsonLinq.Exceptions;
using JsonLinq.Utilities;

namespace JsonLinq.Core;

/// <summary>
/// Provides execution operations over JSON node collections.
/// </summary>
public sealed class QueryEngine(IMatcher matcher)
{
    private readonly IMatcher _matcher = matcher ?? throw new ArgumentNullException(nameof(matcher));

    /// <summary>
    /// Filters nodes by condition.
    /// </summary>
    public IReadOnlyList<JsonNode?> Filter(IEnumerable<JsonNode?> source, JsonCondition condition)
    {
        return source.Where(x => _matcher.IsMatch(x, condition)).ToList().AsReadOnly();
    }

    /// <summary>
    /// Sorts nodes by path and order.
    /// </summary>
    public IReadOnlyList<JsonNode?> Sort(IEnumerable<JsonNode?> source, string path, bool descending = false)
    {
        IOrderedEnumerable<JsonNode?> sorted = descending
            ? source.OrderByDescending(x => JsonValueHelper.ToComparable(PathResolver.Resolve(x, path)))
            : source.OrderBy(x => JsonValueHelper.ToComparable(PathResolver.Resolve(x, path)));

        return sorted.ToList().AsReadOnly();
    }

    /// <summary>
    /// Groups nodes by path.
    /// </summary>
    public IReadOnlyDictionary<string, IReadOnlyList<JsonNode?>> GroupBy(IEnumerable<JsonNode?> source, string path)
    {
        Dictionary<string, IReadOnlyList<JsonNode?>> grouped = source
            .GroupBy(x => JsonValueHelper.ToComparable(PathResolver.Resolve(x, path))?.ToString() ?? string.Empty)
            .ToDictionary(
                x => x.Key,
                x => (IReadOnlyList<JsonNode?>)x.ToList().AsReadOnly(),
                StringComparer.Ordinal);

        return new ReadOnlyDictionary<string, IReadOnlyList<JsonNode?>>(grouped);
    }

    /// <summary>
    /// Returns a sum for a numeric path.
    /// </summary>
    public decimal Sum(IEnumerable<JsonNode?> source, string path)
    {
        IEnumerable<decimal> values = SelectNumbers(source, path);
        return values.Sum();
    }

    /// <summary>
    /// Returns an average for a numeric path.
    /// </summary>
    public decimal Avg(IEnumerable<JsonNode?> source, string path)
    {
        List<decimal> values = SelectNumbers(source, path).ToList();
        return values.Count == 0 ? 0M : values.Average();
    }

    /// <summary>
    /// Returns max value for a numeric path.
    /// </summary>
    public decimal Max(IEnumerable<JsonNode?> source, string path)
    {
        List<decimal> values = SelectNumbers(source, path).ToList();
        return values.Count == 0 ? 0M : values.Max();
    }

    /// <summary>
    /// Returns min value for a numeric path.
    /// </summary>
    public decimal Min(IEnumerable<JsonNode?> source, string path)
    {
        List<decimal> values = SelectNumbers(source, path).ToList();
        return values.Count == 0 ? 0M : values.Min();
    }

    private static IEnumerable<decimal> SelectNumbers(IEnumerable<JsonNode?> source, string path)
    {
        foreach (JsonNode? node in source)
        {
            JsonNode? valueNode = string.IsNullOrWhiteSpace(path) ? node : PathResolver.Resolve(node, path);
            if (valueNode is not JsonValue value)
            {
                continue;
            }

            if (value.TryGetValue<decimal>(out decimal decimalValue))
            {
                yield return decimalValue;
                continue;
            }

            if (value.TryGetValue<double>(out double doubleValue))
            {
                yield return (decimal)doubleValue;
                continue;
            }

            if (value.TryGetValue<long>(out long longValue))
            {
                yield return longValue;
                continue;
            }

            throw new JsonQueryException($"Value at '{path}' is not numeric.");
        }
    }
}
