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
        if (value.TryGetValue<bool>(out var boolValue))
        {
            return boolValue;
        }

        if (value.TryGetValue<long>(out var longValue))
        {
            return longValue;
        }

        if (value.TryGetValue<double>(out var doubleValue))
        {
            return doubleValue;
        }

        if (value.TryGetValue<decimal>(out var decimalValue))
        {
            return decimalValue;
        }

        if (value.TryGetValue<string>(out var stringValue))
        {
            return stringValue;
        }

        return value.ToJsonString();
    }
}
