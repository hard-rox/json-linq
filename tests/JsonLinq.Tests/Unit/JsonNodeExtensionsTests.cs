using JsonLinq.Tests.Fixtures;

namespace JsonLinq.Tests.Unit;

public sealed class JsonNodeExtensionsTests
{
    private readonly SampleDataFixture _fixture = new();

    private JsonNode? AliceNode => JsonNode.Parse(_fixture.Json)?["employees"]?[0];

    // ── Value<T> ──────────────────────────────────────────────────────────────

    [Fact]
    public void Value_ExistingKey_ReturnsCorrectValue()
    {
        Assert.Equal("Alice", AliceNode.Value<string>("name"));
    }

    [Fact]
    public void Value_NumericKey_ReturnsCorrectValue()
    {
        Assert.Equal(32, AliceNode.Value<int>("age"));
    }

    [Fact]
    public void Value_MissingKey_ReturnsDefault()
    {
        Assert.Null(AliceNode.Value<string>("nonexistent"));
    }

    [Fact]
    public void Value_NullNode_ReturnsDefault()
    {
        JsonNode? node = null;
        Assert.Null(node.Value<string>("name"));
    }

    [Fact]
    public void Value_WrongType_ReturnsDefault()
    {
        // "name" is a string; requesting int should return default
        Assert.Equal(default, AliceNode.Value<int>("name"));
    }

    // ── ValueAt<T> ────────────────────────────────────────────────────────────

    [Fact]
    public void ValueAt_NestedPath_ReturnsCorrectValue()
    {
        Assert.Equal("San Francisco", AliceNode.ValueAt<string>("address.city"));
    }

    [Fact]
    public void ValueAt_MissingPath_ReturnsDefault()
    {
        Assert.Null(AliceNode.ValueAt<string>("address.country"));
    }

    [Fact]
    public void ValueAt_NullNode_ReturnsDefault()
    {
        JsonNode? node = null;
        Assert.Null(node.ValueAt<string>("address.city"));
    }
}
