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
        IReadOnlyList<JsonNode?> rows = JsonQuery.Parse(_fixture.Json)
            .From("employees")
            .Where(n => n.Value<string>("department") == "Engineering")
            .OrderByDescending("salary")
            .ToList();

        Assert.Equal(2, rows.Count);
        Assert.Equal("Alice", rows[0]?["name"]?.GetValue<string>());
    }
}
