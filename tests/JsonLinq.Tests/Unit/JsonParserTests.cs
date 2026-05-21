using JsonLinq.Exceptions;
using JsonLinq.Utilities;

namespace JsonLinq.Tests.Unit;

public sealed class JsonParserTests
{
    [Fact]
    public void Parse_ValidJson_ReturnsNode()
    {
        var node = JsonParser.Parse("{\"a\":1}");
        Assert.NotNull(node);
        Assert.Equal(1, node["a"]?.GetValue<int>());
    }

    [Fact]
    public void Parse_InvalidJson_ThrowsJsonQueryException()
    {
        Assert.Throws<JsonQueryException>(() => JsonParser.Parse("not-json"));
    }

    [Fact]
    public void Parse_EmptyString_ThrowsJsonQueryException()
    {
        Assert.Throws<JsonQueryException>(() => JsonParser.Parse(""));
    }

    [Fact]
    public void ParseFile_NonExistentFile_ThrowsFileNotFoundException()
    {
        Assert.Throws<FileNotFoundException>(() => JsonParser.ParseFile("does_not_exist.json"));
    }

    [Fact]
    public async Task ParseFileAsync_NonExistentFile_ThrowsFileNotFoundException()
    {
        await Assert.ThrowsAsync<FileNotFoundException>(() =>
            JsonParser.ParseFileAsync("does_not_exist.json"));
    }
}
