using JsonLinq.Core;

namespace JsonLinq.Tests.Unit;

public sealed class MatcherTests
{
    private readonly Matcher _matcher = new();

    [Theory]
    [InlineData("==", "Ava", true)]
    [InlineData("=",  "Ava", true)]
    [InlineData("==", "Ben", false)]
    [InlineData("!=", "Ava", false)]
    [InlineData("!=", "Ben", true)]
    public void IsMatch_EqualityOperators_WorkAsExpected(string op, string value, bool expected)
    {
        JsonNode? node = JsonNode.Parse("{\"name\":\"Ava\"}");
        Assert.Equal(expected, _matcher.IsMatch(node, new JsonCondition("name", op, value)));
    }

    [Theory]
    [InlineData(">",  20, true)]
    [InlineData(">",  30, false)]
    [InlineData(">=", 30, true)]
    [InlineData(">=", 31, false)]
    [InlineData("<",  31, true)]
    [InlineData("<",  30, false)]
    [InlineData("<=", 30, true)]
    [InlineData("<=", 29, false)]
    public void IsMatch_RelationalOperators_WorkAsExpected(string op, int value, bool expected)
    {
        JsonNode? node = JsonNode.Parse("{\"age\":30}");
        Assert.Equal(expected, _matcher.IsMatch(node, new JsonCondition("age", op, value)));
    }

    [Fact]
    public void IsMatch_RelationalOperator_DecimalVsLong_NoTruncation()
    {
        // salary is stored as long 1000; comparing > 999.9 should be true (not truncated to 999)
        JsonNode? node = JsonNode.Parse("{\"salary\":1000}");
        Assert.True(_matcher.IsMatch(node, new JsonCondition("salary", ">", 999.9)));
        Assert.False(_matcher.IsMatch(node, new JsonCondition("salary", ">", 1000.1)));
    }

    [Theory]
    [InlineData("va",  true)]
    [InlineData("Ava", true)]
    [InlineData("AVA", true)]   // case-insensitive
    [InlineData("xyz", false)]
    public void IsMatch_ContainsOperator_WorksAsExpected(string substring, bool expected)
    {
        JsonNode? node = JsonNode.Parse("{\"name\":\"Ava\"}");
        Assert.Equal(expected, _matcher.IsMatch(node, new JsonCondition("name", "contains", substring)));
    }

    [Fact]
    public void IsMatch_InOperator_WithJsonArray_MatchesPresent()
    {
        JsonNode? node = JsonNode.Parse("{\"dept\":\"Engineering\"}");
        JsonNode? allowedSet = JsonNode.Parse("[\"Engineering\",\"Sales\"]");
        Assert.True(_matcher.IsMatch(node, new JsonCondition("dept", "in", allowedSet)));
    }

    [Fact]
    public void IsMatch_InOperator_WithJsonArray_ReturnsFalseWhenAbsent()
    {
        JsonNode? node = JsonNode.Parse("{\"dept\":\"Marketing\"}");
        JsonNode? allowedSet = JsonNode.Parse("[\"Engineering\",\"Sales\"]");
        Assert.False(_matcher.IsMatch(node, new JsonCondition("dept", "in", allowedSet)));
    }

    [Fact]
    public void IsMatch_MissingField_ReturnsFalse()
    {
        JsonNode? node = JsonNode.Parse("{\"name\":\"Ava\"}");
        Assert.False(_matcher.IsMatch(node, new JsonCondition("nonexistent", "==", "x")));
    }
}
