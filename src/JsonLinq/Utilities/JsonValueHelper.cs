namespace JsonLinq.Utilities;

internal static class JsonValueHelper
{
    public static object? ToComparable(object? value)
    {
        return value switch
        {
            JsonValue jsonValue => ConvertJsonValue(jsonValue),
            JsonNode node => node.ToJsonString(),
            _ => value
        };
    }

    private static object ConvertJsonValue(JsonValue value)
    {
        if (value.TryGetValue(out bool boolValue))
        {
            return boolValue;
        }

        if (value.TryGetValue(out long longValue))
        {
            return longValue;
        }

        if (value.TryGetValue(out double doubleValue))
        {
            return doubleValue;
        }

        if (value.TryGetValue(out decimal decimalValue))
        {
            return decimalValue;
        }

        if (value.TryGetValue(out string? stringValue))
        {
            return stringValue;
        }

        return value.ToJsonString();
    }
}
