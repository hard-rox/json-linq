using JsonLinq.Core;

namespace JsonLinq.Tests.Unit;

public sealed class MatcherTests
{
    private static readonly string _nameData = "{\"items\":[{\"name\":\"Ava\"}]}";
    private static readonly string _ageData  = "{\"items\":[{\"age\":30}]}";

    [Theory]
    [InlineData("==", "Ava", true)]
    [InlineData("=",  "Ava", true)]
    [InlineData("==", "Ben", false)]
    [InlineData("!=", "Ava", false)]
    [InlineData("!=", "Ben", true)]
    public void Where_EqualityOperators_WorkAsExpected(string op, string value, bool expected)
    {
        JsonQuery query = JsonQuery.Parse(_nameData).From("items");
        int count = query.Where("name", op, value).Get().Count;
        Assert.Equal(expected ? 1 : 0, count);
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
    public void Where_RelationalOperators_WorkAsExpected(string op, int value, bool expected)
    {
        JsonQuery query = JsonQuery.Parse(_ageData).From("items");
        int count = query.Where("age", op, value).Get().Count;
        Assert.Equal(expected ? 1 : 0, count);
    }

    [Fact]
    public void Where_RelationalOperator_DecimalVsLong_NoTruncation()
    {
        JsonQuery query = JsonQuery.Parse("{\"items\":[{\"salary\":1000}]}").From("items");
        Assert.Single(query.Where("salary", ">", 999.9).Get());
        Assert.Empty(query.Where("salary", ">", 1000.1).Get());
    }

    [Theory]
    [InlineData("va",  true)]
    [InlineData("Ava", true)]
    [InlineData("AVA", true)]
    [InlineData("xyz", false)]
    public void Where_ContainsOperator_WorksAsExpected(string substring, bool expected)
    {
        JsonQuery query = JsonQuery.Parse(_nameData).From("items");
        int count = query.Where("name", "contains", substring).Get().Count;
        Assert.Equal(expected ? 1 : 0, count);
    }

    [Fact]
    public void Where_InOperator_WithJsonArray_MatchesPresent()
    {
        JsonQuery query = JsonQuery.Parse("{\"items\":[{\"dept\":\"Engineering\"}]}").From("items");
        JsonNode? allowedSet = JsonNode.Parse("[\"Engineering\",\"Sales\"]");
        Assert.Single(query.Where("dept", "in", allowedSet).Get());
    }

    [Fact]
    public void Where_InOperator_WithJsonArray_ReturnsFalseWhenAbsent()
    {
        JsonQuery query = JsonQuery.Parse("{\"items\":[{\"dept\":\"Marketing\"}]}").From("items");
        JsonNode? allowedSet = JsonNode.Parse("[\"Engineering\",\"Sales\"]");
        Assert.Empty(query.Where("dept", "in", allowedSet).Get());
    }

    [Fact]
    public void Where_MissingField_ReturnsFalse()
    {
        JsonQuery query = JsonQuery.Parse(_nameData).From("items");
        Assert.Empty(query.Where("nonexistent", "==", "x").Get());
    }
}
