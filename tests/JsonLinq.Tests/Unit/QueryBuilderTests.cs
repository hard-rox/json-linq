using JsonLinq.Core;
using JsonLinq.Tests.Fixtures;

namespace JsonLinq.Tests.Unit;

public sealed class QueryBuilderTests
{
    private readonly SampleDataFixture _fixture = new();

    [Fact]
    public void WhereAndCount_ReturnsExpectedCount()
    {
        var count = JsonQuery.Parse(_fixture.Json)
            .From("users")
            .Where("department", "==", "Engineering")
            .Count();

        Assert.Equal(2, count);
    }

    [Fact]
    public void SumAndAvg_ReturnExpectedValues()
    {
        var query = JsonQuery.Parse(_fixture.Json).From("users");

        Assert.Equal(2700M, query.Sum("salary"));
        Assert.Equal(900M, query.Average("salary"));
    }

    [Fact]
    public void GroupBy_ReturnsGroupedRows()
    {
        var grouped = JsonQuery.Parse(_fixture.Json)
            .From("users")
            .GroupBy("department")
            .Get();

        Assert.Equal(2, grouped.Count);
    }
}
