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
        IReadOnlyList<JsonNode?> rows = JsonQuery.Parse(_fixture.Json)
            .From("employees")
            .Where("department", "==", "Engineering")
            .OrderByDescending("salary")
            .Get();

        Assert.Equal(2, rows.Count);
        Assert.Equal("Alice", rows[0]?["name"]?.GetValue<string>());
    }
}
