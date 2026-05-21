namespace JsonLinq.Core;

/// <summary>
/// Matches JSON items against conditions.
/// </summary>
public interface IMatcher
{
    /// <summary>
    /// Returns true when an item satisfies the condition.
    /// </summary>
    bool IsMatch(JsonNode? item, JsonCondition condition);
}
