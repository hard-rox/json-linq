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

    private static object? ConvertJsonValue(JsonValue value)
    {
        if (value.TryGetValue<bool>(out bool boolValue))
        {
            return boolValue;
        }

        if (value.TryGetValue<long>(out long longValue))
        {
            return longValue;
        }

        if (value.TryGetValue<double>(out double doubleValue))
        {
            return doubleValue;
        }

        if (value.TryGetValue<decimal>(out decimal decimalValue))
        {
            return decimalValue;
        }

        if (value.TryGetValue<string>(out string? stringValue))
        {
            return stringValue;
        }

        return value.ToJsonString();
    }
}
