using JsonLinq.Exceptions;

namespace JsonLinq.Utilities;

/// <summary>
/// Resolves dot-notated paths over JSON nodes.
/// </summary>
public static class PathResolver
{
    /// <summary>
    /// Resolves a node by path.
    /// </summary>
    /// <param name="root">Root node.</param>
    /// <param name="path">Dot-notated path like users.0.name.</param>
    /// <returns>The resolved node or null if not found.</returns>
    public static JsonNode? Resolve(JsonNode? root, string path)
    {
        if (root is null)
        {
            return null;
        }

        if (string.IsNullOrWhiteSpace(path))
        {
            return root;
        }

        JsonNode? current = root;
        foreach (var segment in path.Split('.', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            if (current is JsonObject obj)
            {
                current = obj[segment];
                continue;
            }

            if (current is JsonArray arr)
            {
                if (!int.TryParse(segment, out var index))
                {
                    throw new InvalidPathException($"Path segment '{segment}' is not a valid array index.");
                }

                current = index >= 0 && index < arr.Count ? arr[index] : null;
                continue;
            }

            return null;
        }

        return current;
    }
}
