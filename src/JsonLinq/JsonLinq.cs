using JsonLinq.Core;

namespace JsonLinq;

/// <summary>
/// Factory entry point for JsonLinq queries.
/// </summary>
public static class JsonLinq
{
    /// <summary>
    /// Creates a query from JSON string content.
    /// </summary>
    public static JsonQuery Parse(string json) => JsonQuery.Parse(json);

    /// <summary>
    /// Creates a query from a JSON file path.
    /// </summary>
    public static JsonQuery ParseFile(string path) => JsonQuery.ParseFile(path);

    /// <summary>
    /// Asynchronously creates a query from a JSON file path.
    /// </summary>
    public static Task<JsonQuery> ParseFileAsync(string path, CancellationToken cancellationToken = default) =>
        JsonQuery.ParseFileAsync(path, cancellationToken);
}
