using JsonLinq.Utilities;

namespace JsonLinq.Core;

/// <summary>
/// Fluent query object for JSON data.
/// </summary>
public sealed class JsonQuery
{
    private readonly JsonNode _root;
    private readonly IReadOnlyList<JsonNode?> _scope;
    private readonly IReadOnlyList<JsonNode?> _result;
    private readonly QueryEngine _engine;

    private JsonQuery(JsonNode root, IReadOnlyList<JsonNode?> scope, IReadOnlyList<JsonNode?> result, QueryEngine engine)
    {
        _root = root;
        _scope = scope;
        _result = result;
        _engine = engine;
    }

    /// <summary>
    /// Creates a query from JSON text.
    /// </summary>
    public static JsonQuery Parse(string json)
    {
        JsonNode root = JsonParser.Parse(json);
        ReadOnlyCollection<JsonNode?> seed = new List<JsonNode?> { root }.AsReadOnly();
        return new JsonQuery(root, seed, seed, new QueryEngine());
    }

    /// <summary>
    /// Creates a query from a JSON file.
    /// </summary>
    public static JsonQuery ParseFile(string filePath)
    {
        JsonNode root = JsonParser.ParseFile(filePath);
        ReadOnlyCollection<JsonNode?> seed = new List<JsonNode?> { root }.AsReadOnly();
        return new JsonQuery(root, seed, seed, new QueryEngine());
    }

    /// <summary>
    /// Asynchronously creates a query from a JSON file.
    /// </summary>
    public static async Task<JsonQuery> ParseFileAsync(string filePath, CancellationToken cancellationToken = default)
    {
        JsonNode root = await JsonParser.ParseFileAsync(filePath, cancellationToken).ConfigureAwait(false);
        ReadOnlyCollection<JsonNode?> seed = new List<JsonNode?> { root }.AsReadOnly();
        return new JsonQuery(root, seed, seed, new QueryEngine());
    }

    /// <inheritdoc />
    public JsonQuery From(string path)
    {
        JsonNode? node = PathResolver.Resolve(_root, path);
        ReadOnlyCollection<JsonNode?> collection = ToCollection(node);
        return new JsonQuery(_root, collection, collection, _engine);
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
    public JsonQuery Where(string field, string op, object? value)
    {
        IReadOnlyList<JsonNode?> filtered = _result.Where(n => JsonConditionEvaluator.Evaluate(n, field, op, value)).ToList().AsReadOnly();
        return new JsonQuery(_root, _scope, filtered, _engine);
    }

    /// <summary>
    /// Unions the current results with nodes from scope matching a predicate.
    /// </summary>
    public JsonQuery OrWhere(string field, string op, object? value)
    {
        IReadOnlyList<JsonNode?> filteredFromScope = _scope.Where(n => JsonConditionEvaluator.Evaluate(n, field, op, value)).ToList().AsReadOnly();
        ReadOnlyCollection<JsonNode?> union = _result
            .Concat(filteredFromScope)
            .DistinctBy(x => x?.ToJsonString() ?? string.Empty)
            .ToList()
            .AsReadOnly();

        return new JsonQuery(_root, _scope, union, _engine);
    }

    /// <summary>
    /// Sorts current results by a field path in ascending order.
    /// </summary>
    public JsonQuery OrderBy(string field)
    {
        IReadOnlyList<JsonNode?> sorted = _engine.Sort(_result, field, descending: false);
        return new JsonQuery(_root, _scope, sorted, _engine);
    }

    /// <summary>
    /// Sorts current results by a field path in descending order.
    /// </summary>
    public JsonQuery OrderByDescending(string field)
    {
        IReadOnlyList<JsonNode?> sorted = _engine.Sort(_result, field, descending: true);
        return new JsonQuery(_root, _scope, sorted, _engine);
    }

    /// <summary>
    /// Returns grouped data represented as objects with key/items fields.
    /// </summary>
    public JsonQuery GroupBy(string field)
    {
        IReadOnlyDictionary<string, IReadOnlyList<JsonNode?>> grouped = _engine.GroupBy(_result, field);
        ReadOnlyCollection<JsonNode?> nodes = grouped
            .Select(x => new JsonObject
            {
                ["key"] = x.Key,
                ["items"] = new JsonArray(x.Value.Select(v => v?.DeepClone()).ToArray())
            })
            .Cast<JsonNode?>()
            .ToList()
            .AsReadOnly();

        return new JsonQuery(_root, nodes, nodes, _engine);
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
        return new JsonQuery(_root, readOnly, readOnly, _engine);
    }

    /// <summary>
    /// Gets a deep copy of this query.
    /// </summary>
    public JsonQuery Copy()
    {
        JsonNode newRoot = _root.DeepClone();
        ReadOnlyCollection<JsonNode?> newScope = _scope.Select(x => x?.DeepClone()).ToList().AsReadOnly();
        ReadOnlyCollection<JsonNode?> newResult = _result.Select(x => x?.DeepClone()).ToList().AsReadOnly();
        return new JsonQuery(newRoot, newScope, newResult, _engine);
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

        return new JsonQuery(_root, _scope, distinct, _engine);
    }

    /// <summary>
    /// Returns true when query has at least one result.
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
    public decimal Sum(string field) => _engine.Sum(_result, field);

    /// <summary>
    /// Returns the average value for a numeric field.
    /// </summary>
    public decimal Average(string field) => _engine.Avg(_result, field);

    /// <summary>
    /// Returns max value for field.
    /// </summary>
    public decimal Max(string field) => _engine.Max(_result, field);

    /// <summary>
    /// Returns min value for field.
    /// </summary>
    public decimal Min(string field) => _engine.Min(_result, field);

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

        return new JsonQuery(_root, projected, projected, _engine);
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
        return new JsonQuery(_root, _scope, taken, _engine);
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
        return new JsonQuery(_root, _scope, remaining, _engine);
    }

    /// <inheritdoc />
    public IReadOnlyList<JsonNode?> Get() => _result;

    private static ReadOnlyCollection<JsonNode?> ToCollection(JsonNode? node)
    {
        if (node is JsonArray arr)
        {
            return arr.Select(x => x).ToList().AsReadOnly();
        }

        return new List<JsonNode?> { node }.AsReadOnly();
    }
}
