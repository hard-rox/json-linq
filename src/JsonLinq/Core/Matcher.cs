using JsonLinq.Utilities;

namespace JsonLinq.Core;

/// <summary>
/// Default condition matcher implementation.
/// </summary>
public sealed class Matcher : IMatcher
{
    /// <inheritdoc />
    public bool IsMatch(JsonNode? item, JsonCondition condition)
    {
        var node = PathResolver.Resolve(item, condition.Field);
        var left = JsonValueHelper.ToComparable(node);
        var right = JsonValueHelper.ToComparable(condition.Value);
        var op = condition.Operator.Trim().ToLowerInvariant();

        return op switch
        {
            "=" or "==" => Equals(left, right),
            "!=" or "<>" => !Equals(left, right),
            ">" => Compare(left, right) > 0,
            ">=" => Compare(left, right) >= 0,
            "<" => Compare(left, right) < 0,
            "<=" => Compare(left, right) <= 0,
            "contains" => left?.ToString()?.Contains(right?.ToString() ?? string.Empty, StringComparison.OrdinalIgnoreCase) == true,
            // Use condition.Value directly: ToComparable converts JsonArray to a string, breaking membership tests
            "in" => IsIn(condition.Value, left),
            _ => false
        };
    }

    private static int Compare(object? left, object? right)
    {
        if (left is null && right is null) return 0;
        if (left is null) return -1;
        if (right is null) return 1;

        // Normalize both sides to decimal when either is numeric to prevent
        // Convert.ChangeType truncation (e.g. double 999.9 -> long 999)
        if (IsNumeric(left) && IsNumeric(right))
        {
            var l = Convert.ToDecimal(left);
            var r = Convert.ToDecimal(right);
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
