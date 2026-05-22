using JsonLinq.Utilities;

namespace JsonLinq.Core;

/// <summary>
/// Evaluates string-based conditions against JSON nodes.
/// </summary>
internal static class JsonConditionEvaluator
{
    internal static bool Evaluate(JsonNode? item, string field, string op, object? value)
    {
        JsonNode? node = PathResolver.Resolve(item, field);
        object? left = JsonValueHelper.ToComparable(node);
        object? right = JsonValueHelper.ToComparable(value);
        string normalizedOp = op.Trim().ToLowerInvariant();

        return normalizedOp switch
        {
            "=" or "==" => Equals(left, right),
            "!=" or "<>" => !Equals(left, right),
            ">" => Compare(left, right) > 0,
            ">=" => Compare(left, right) >= 0,
            "<" => Compare(left, right) < 0,
            "<=" => Compare(left, right) <= 0,
            "contains" => left?.ToString()?.Contains(right?.ToString() ?? string.Empty, StringComparison.OrdinalIgnoreCase) == true,
            "in" => IsIn(value, left),
            _ => false
        };
    }

    private static int Compare(object? left, object? right)
    {
        if (left is null && right is null) return 0;
        if (left is null) return -1;
        if (right is null) return 1;

        if (IsNumeric(left) && IsNumeric(right))
        {
            decimal l = Convert.ToDecimal(left);
            decimal r = Convert.ToDecimal(right);
            return l.CompareTo(r);
        }

        if (left is IComparable comparable)
        {
            try
            {
                return comparable.CompareTo(Convert.ChangeType(right, left.GetType()));
            }
            catch (InvalidCastException)
            {
                return string.CompareOrdinal(left.ToString(), right.ToString());
            }
        }

        return string.CompareOrdinal(left.ToString(), right.ToString());
    }

    private static bool IsNumeric(object value) =>
        value is byte or sbyte or short or ushort or int or uint
            or long or ulong or float or double or decimal;

    private static bool IsIn(object? candidateSet, object? value)
    {
        if (candidateSet is JsonArray jsonArray)
        {
            return jsonArray.Any(x => Equals(JsonValueHelper.ToComparable(x), value));
        }

        if (candidateSet is IEnumerable<object?> values)
        {
            return values.Any(x => Equals(x, value));
        }

        return false;
    }
}
