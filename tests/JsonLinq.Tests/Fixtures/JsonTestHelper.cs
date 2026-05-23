namespace JsonLinq.Tests.Fixtures;

public static class JsonTestHelper
{
    public static string? GetString(JsonNode? node, string key)
    {
        return node?[key]?.GetValue<string>();
    }

    public static decimal GetDecimal(JsonNode? node, string key)
    {
        return node?[key]?.GetValue<decimal>() ?? 0M;
    }
}
