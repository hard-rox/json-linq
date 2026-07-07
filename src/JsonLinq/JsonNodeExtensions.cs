using JsonLinq.Utilities;

namespace JsonLinq;

/// <summary>
/// Extension methods on <see cref="JsonNode"/> for concise value access inside predicates.
/// </summary>
public static class JsonNodeExtensions
{
    /// <summary>
    /// Returns the value of a direct child key, or <c>default</c> if the key is missing or the type does not match.
    /// </summary>
    public static T? Value<T>(this JsonNode? node, string key)
    {
        try
        {
            return node?[key] is { } n ? n.GetValue<T>() : default;
        }
        catch (InvalidOperationException)
        {
            return default;
        }
    }

    /// <summary>
    /// Returns the value at a dot-notated nested path, or <c>default</c> if the path is missing or the type does not match.
    /// </summary>
    public static T? ValueAt<T>(this JsonNode? node, string path)
    {
        try
        {
            return PathResolver.Resolve(node, path) is { } n ? n.GetValue<T>() : default;
        }
        catch (InvalidOperationException)
        {
            return default;
        }
    }
}