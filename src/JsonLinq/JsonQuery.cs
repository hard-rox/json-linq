using JsonLinq.Exceptions;
using JsonLinq.Utilities;

namespace JsonLinq;

/// <summary>
/// Fluent query object for JSON data.
/// </summary>
public sealed class JsonQuery
{
    private readonly JsonNode _root;
    private readonly IReadOnlyList<JsonNode?> _scope;
    private readonly IReadOnlyList<JsonNode?> _result;

    private JsonQuery(JsonNode root, IReadOnlyList<JsonNode?> scope, IReadOnlyList<JsonNode?> result)
    {
        _root = root;
        _scope = scope;
        _result = result;
    }

    /// <summary>
    /// Creates a query from JSON text.
    /// </summary>
    public static JsonQuery Parse(string json)
    {
        JsonNode root = JsonParser.Parse(json);
        ReadOnlyCollection<JsonNode?> seed = new List<JsonNode?> { root }.AsReadOnly();
        return new JsonQuery(root, seed, seed);
    }

    /// <summary>
    /// Creates a query from a JSON file.
    /// </summary>
    public static JsonQuery ParseFile(string filePath)
    {
        JsonNode root = JsonParser.ParseFile(filePath);
        ReadOnlyCollection<JsonNode?> seed = new List<JsonNode?> { root }.AsReadOnly();
        return new JsonQuery(root, seed, seed);
    }

    /// <summary>
    /// Asynchronously creates a query from a JSON file.
    /// </summary>
    public static async Task<JsonQuery> ParseFileAsync(string filePath, CancellationToken cancellationToken = default)
    {
        JsonNode root = await JsonParser.ParseFileAsync(filePath, cancellationToken).ConfigureAwait(false);
        ReadOnlyCollection<JsonNode?> seed = new List<JsonNode?> { root }.AsReadOnly();
        return new JsonQuery(root, seed, seed);
    }

    /// <summary>
    /// Creates a subset of the query by resolving the specified path from the root node.
    /// </summary>
    /// <param name="path">The path to resolve within the JSON structure.</param>
    /// <returns>A new <see cref="JsonQuery"/> instance scoped to the resolved subset.</returns>
    public JsonQuery From(string path)
    {
        JsonNode? node = PathResolver.Resolve(_root, path);
        ReadOnlyCollection<JsonNode?> collection;
        if (node is JsonArray arr)
        {
            collection = arr.Select(x => x).ToList().AsReadOnly();
        }
        else
        {
            collection = new List<JsonNode?> { node }.AsReadOnly();
        }

        return new JsonQuery(_root, collection, collection);
    }

    /// <summary>
    /// Finds a single node by path.
    /// </summary>
    public JsonNode? Find(string path) => PathResolver.Resolve(_root, path);

    /// <summary>
    /// Alias for <see cref="Find"/>.
    /// </summary>
    public JsonNode? At(string path) => Find(path);

    /// <summary>
    /// Filters the current results using a predicate.
    /// </summary>
    public JsonQuery Where(Func<JsonNode?, bool> predicate)
    {
        IReadOnlyList<JsonNode?> filtered = _result.Where(predicate).ToList().AsReadOnly();
        return new JsonQuery(_root, _scope, filtered);
    }

    /// <summary>
    /// Sorts current results by a field path in ascending order.
    /// </summary>
    public JsonQuery OrderBy(string field)
    {
        IReadOnlyList<JsonNode?> sorted = Sort(_result, field, descending: false);
        return new JsonQuery(_root, _scope, sorted);
    }

    /// <summary>
    /// Sorts current results by a field path in descending order.
    /// </summary>
    public JsonQuery OrderByDescending(string field)
    {
        IReadOnlyList<JsonNode?> sorted = Sort(_result, field, descending: true);
        return new JsonQuery(_root, _scope, sorted);
    }

    /// <summary>
    /// Returns grouped data represented as objects with key/items fields.
    /// </summary>
    public JsonQuery GroupBy(string field)
    {
        IReadOnlyDictionary<string, IReadOnlyList<JsonNode?>> grouped = GroupByField(_result, field);
        ReadOnlyCollection<JsonNode?> nodes = grouped
            .Select(x => new JsonObject
            {
                ["key"] = x.Key,
                ["items"] = new JsonArray(x.Value.Select(v => v?.DeepClone()).ToArray())
            })
            .Cast<JsonNode?>()
            .ToList()
            .AsReadOnly();

        return new JsonQuery(_root, nodes, nodes);
    }

    /// <summary>
    /// Breaks current results into chunks.
    /// </summary>
    public JsonQuery Chunk(int size)
    {
        if (size <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(size), "Chunk size must be greater than zero.");
        }

        List<JsonNode?> list = new List<JsonNode?>();
        for (int i = 0; i < _result.Count; i += size)
        {
            list.Add(new JsonArray(_result.Skip(i).Take(size).Select(x => x?.DeepClone()).ToArray()));
        }

        ReadOnlyCollection<JsonNode?> readOnly = list.AsReadOnly();
        return new JsonQuery(_root, readOnly, readOnly);
    }

    /// <summary>
    /// Gets a deep copy of this query.
    /// </summary>
    public JsonQuery Copy()
    {
        JsonNode newRoot = _root.DeepClone();
        ReadOnlyCollection<JsonNode?> newScope = _scope.Select(x => x?.DeepClone()).ToList().AsReadOnly();
        ReadOnlyCollection<JsonNode?> newResult = _result.Select(x => x?.DeepClone()).ToList().AsReadOnly();
        return new JsonQuery(newRoot, newScope, newResult);
    }

    /// <summary>
    /// Removes duplicate items.
    /// </summary>
    public JsonQuery Distinct()
    {
        ReadOnlyCollection<JsonNode?> distinct = _result
            .DistinctBy(x => x?.ToJsonString() ?? string.Empty)
            .ToList()
            .AsReadOnly();

        return new JsonQuery(_root, _scope, distinct);
    }

    /// <summary>
    /// Returns true when a query has at least one result.
    /// </summary>
    public bool Exists() => _result.Count > 0;

    /// <summary>
    /// Returns the first element of the sequence, or null if the sequence is empty.
    /// </summary>
    public JsonNode? FirstOrDefault() => _result.FirstOrDefault();

    /// <summary>
    /// Returns the last element of the sequence, or null if the sequence is empty.
    /// </summary>
    public JsonNode? LastOrDefault() => _result.LastOrDefault();

    /// <summary>
    /// Returns the only element of the sequence, or null if the sequence is empty.
    /// Throws <see cref="InvalidOperationException"/> if more than one element exists.
    /// </summary>
    public JsonNode? SingleOrDefault()
    {
        return _result.Count switch
        {
            0 => null,
            1 => _result[0],
            _ => throw new InvalidOperationException("Sequence contains more than one element.")
        };
    }

    /// <summary>
    /// Returns result at index or null.
    /// </summary>
    public JsonNode? Nth(int index) => index >= 0 && index < _result.Count ? _result[index] : null;

    /// <summary>
    /// Returns item count.
    /// </summary>
    public int Count() => _result.Count;

    /// <summary>
    /// Returns sum value for field.
    /// </summary>
    public decimal Sum(string field) => SumNumbers(_result, field);

    /// <summary>
    /// Returns the average value for a numeric field.
    /// </summary>
    public decimal Average(string field)
    {
        List<decimal> values = SelectNumbers(_result, field).ToList();
        return values.Count == 0 ? 0M : values.Average();
    }

    /// <summary>
    /// Returns max value for a field.
    /// </summary>
    public decimal Max(string field)
    {
        List<decimal> values = SelectNumbers(_result, field).ToList();
        return values.Count == 0 ? 0M : values.Max();
    }

    /// <summary>
    /// Returns min value for field.
    /// </summary>
    public decimal Min(string field)
    {
        List<decimal> values = SelectNumbers(_result, field).ToList();
        return values.Count == 0 ? 0M : values.Min();
    }

    /// <summary>
    /// Projects each result node to a new object containing only the specified fields.
    /// Missing fields are omitted silently.
    /// </summary>
    public JsonQuery Select(params string[] fields)
    {
        ReadOnlyCollection<JsonNode?> projected = _result
            .Select(node =>
            {
                JsonObject obj = new JsonObject();
                foreach (string field in fields)
                {
                    JsonNode? value = PathResolver.Resolve(node, field);
                    if (value is not null)
                    {
                        obj[field] = value.DeepClone();
                    }
                }

                return (JsonNode?)obj;
            })
            .ToList()
            .AsReadOnly();

        return new JsonQuery(_root, projected, projected);
    }

    /// <summary>
    /// Returns at most <paramref name="count"/> results from the start.
    /// </summary>
    public JsonQuery Take(int count)
    {
        if (count < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(count), "Take count must be non-negative.");
        }

        ReadOnlyCollection<JsonNode?> taken = _result.Take(count).ToList().AsReadOnly();
        return new JsonQuery(_root, _scope, taken);
    }

    /// <summary>
    /// Skips the first <paramref name="count"/> results.
    /// </summary>
    public JsonQuery Skip(int count)
    {
        if (count < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(count), "Skip count must be non-negative.");
        }

        ReadOnlyCollection<JsonNode?> remaining = _result.Skip(count).ToList().AsReadOnly();
        return new JsonQuery(_root, _scope, remaining);
    }

    /// <summary>
    /// Converts the query result into a read-only list of JSON nodes.
    /// </summary>
    /// <returns>
    /// A read-only list containing the JSON nodes that match the query result.
    /// </returns>
    public IReadOnlyList<JsonNode?> ToList() => _result;

    // ── Private static helpers (inlined from QueryEngine) ─────────────────────

    private static IReadOnlyList<JsonNode?> Sort(IEnumerable<JsonNode?> source, string path, bool descending)
    {
        IOrderedEnumerable<JsonNode?> sorted = descending
            ? source.OrderByDescending(x => JsonValueHelper.ToComparable(PathResolver.Resolve(x, path)))
            : source.OrderBy(x => JsonValueHelper.ToComparable(PathResolver.Resolve(x, path)));

        return sorted.ToList().AsReadOnly();
    }

    private static IReadOnlyDictionary<string, IReadOnlyList<JsonNode?>> GroupByField(IEnumerable<JsonNode?> source, string path)
    {
        Dictionary<string, IReadOnlyList<JsonNode?>> grouped = source
            .GroupBy(x => JsonValueHelper.ToComparable(PathResolver.Resolve(x, path))?.ToString() ?? string.Empty)
            .ToDictionary(
                x => x.Key,
                x => (IReadOnlyList<JsonNode?>)x.ToList().AsReadOnly(),
                StringComparer.Ordinal);

        return new ReadOnlyDictionary<string, IReadOnlyList<JsonNode?>>(grouped);
    }

    private static decimal SumNumbers(IEnumerable<JsonNode?> source, string path) =>
        SelectNumbers(source, path).Sum();

    private static IEnumerable<decimal> SelectNumbers(IEnumerable<JsonNode?> source, string path)
    {
        foreach (JsonNode? node in source)
        {
            JsonNode? valueNode = string.IsNullOrWhiteSpace(path) ? node : PathResolver.Resolve(node, path);
            if (valueNode is not JsonValue value)
            {
                continue;
            }

            if (value.TryGetValue(out decimal decimalValue))
            {
                yield return decimalValue;
                continue;
            }

            if (value.TryGetValue(out double doubleValue))
            {
                yield return (decimal)doubleValue;
                continue;
            }

            if (value.TryGetValue(out long longValue))
            {
                yield return longValue;
                continue;
            }

            throw new JsonQueryException($"Value at '{path}' is not numeric.");
        }
    }
}