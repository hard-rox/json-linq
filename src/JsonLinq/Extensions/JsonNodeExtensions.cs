using JsonLinq.Utilities;

namespace JsonLinq.Extensions;

/// <summary>
/// Extension methods on <see cref="JsonNode"/> for concise value access inside predicates.
/// </summary>
public static class JsonNodeExtensions
{
    extension(JsonNode? node)
    {
        /// <summary>
        /// Returns the value of a direct child key, or <c>default</c> if the key is missing or the type does not match.
        /// </summary>
        public T? Value<T>(string key)
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
        public T? ValueAt<T>(string path)
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
}