using JsonLinq.Core;
using JsonLinq.Tests.Fixtures;

namespace JsonLinq.Tests.Unit;

public sealed class QueryBuilderTests
{
    private readonly SampleDataFixture _fixture = new();

    [Fact]
    public void WhereAndCount_ReturnsExpectedCount()
    {
        int count = JsonQuery.Parse(_fixture.Json)
            .From("employees")
            .Where("department", "==", "Engineering")
            .Count();

        Assert.Equal(2, count);
    }

    [Fact]
    public void SumAndAvg_ReturnExpectedValues()
    {
        JsonQuery query = JsonQuery.Parse(_fixture.Json).From("employees");

        Assert.Equal(240000M, query.Sum("salary"));
        Assert.Equal(80000M, query.Average("salary"));
    }

    [Fact]
    public void GroupBy_ReturnsGroupedRows()
    {
        IReadOnlyList<JsonNode?> grouped = JsonQuery.Parse(_fixture.Json)
            .From("employees")
            .GroupBy("department")
            .Get();

        Assert.Equal(2, grouped.Count);
    }
}
