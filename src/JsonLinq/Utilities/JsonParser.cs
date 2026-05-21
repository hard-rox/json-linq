using JsonLinq.Exceptions;

namespace JsonLinq.Utilities;

/// <summary>
/// Parses JSON from text or file sources.
/// </summary>
public static class JsonParser
{
    /// <summary>
    /// Parses JSON text into a <see cref="JsonNode"/>.
    /// </summary>
    /// <param name="json">Raw JSON input.</param>
    /// <returns>The parsed JSON node.</returns>
    public static JsonNode Parse(string json)
    {
        try
        {
            return JsonNode.Parse(json) ?? throw new JsonQueryException("Input JSON was empty.");
        }
        catch (JsonException ex)
        {
            throw new JsonQueryException("Input is not valid JSON.", ex);
        }
    }

    /// <summary>
    /// Parses JSON from a file path.
    /// </summary>
    /// <param name="path">Path to a JSON file.</param>
    /// <returns>The parsed JSON node.</returns>
    public static JsonNode ParseFile(string path)
    {
        if (!File.Exists(path))
        {
            throw new FileNotFoundException($"JSON file not found: {path}", path);
        }

        return Parse(File.ReadAllText(path));
    }

    /// <summary>
    /// Asynchronously parses JSON from a file path.
    /// </summary>
    /// <param name="path">Path to a JSON file.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The parsed JSON node.</returns>
    public static async Task<JsonNode> ParseFileAsync(string path, CancellationToken cancellationToken = default)
    {
        if (!File.Exists(path))
        {
            throw new FileNotFoundException($"JSON file not found: {path}", path);
        }

        string text = await File.ReadAllTextAsync(path, cancellationToken).ConfigureAwait(false);
        return Parse(text);
    }
}
