using JsonLinq.Core;
using JsonLinq.Tests.Fixtures;

namespace JsonLinq.Tests.Integration;

public sealed class EndToEndTests : IClassFixture<SampleDataFixture>
{
    private readonly SampleDataFixture _fixture;

    public EndToEndTests(SampleDataFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void ComplexQueryChain_ReturnsExpectedRows()
    {
        var rows = JsonQuery.Parse(_fixture.Json)
            .From("users")
            .Where("department", "==", "Engineering")
            .OrderByDescending("salary")
            .Get();

        Assert.Equal(2, rows.Count);
        Assert.Equal("Ava", rows[0]?["name"]?.GetValue<string>());
    }
}
