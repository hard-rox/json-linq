using JsonLinq.Exceptions;
using JsonLinq.Utilities;

namespace JsonLinq.Tests.Unit;

public sealed class PathResolverTests
{
    private static readonly JsonNode Root = JsonNode.Parse("""
        { "users": [ { "name": "Ava" }, { "name": "Ben" } ] }
        """)!;

    [Theory]
    [InlineData("users.0.name", "Ava")]
    [InlineData("users.1.name", "Ben")]
    public void Resolve_ValidPath_ReturnsNode(string path, string expected)
    {
        var result = PathResolver.Resolve(Root, path);

        Assert.NotNull(result);
        Assert.Equal(expected, result!.GetValue<string>());
    }

    [Fact]
    public void Resolve_EmptyPath_ReturnsRoot()
    {
        var result = PathResolver.Resolve(Root, "");
        Assert.Same(Root, result);
    }

    [Fact]
    public void Resolve_MissingKey_ReturnsNull()
    {
        var result = PathResolver.Resolve(Root, "nonexistent");
        Assert.Null(result);
    }

    [Fact]
    public void Resolve_ArrayOutOfBounds_ReturnsNull()
    {
        var result = PathResolver.Resolve(Root, "users.99");
        Assert.Null(result);
    }

    [Fact]
    public void Resolve_NullRoot_ReturnsNull()
    {
        var result = PathResolver.Resolve(null, "users");
        Assert.Null(result);
    }

    [Fact]
    public void Resolve_NonNumericArraySegment_ThrowsInvalidPathException()
    {
        Assert.Throws<InvalidPathException>(() =>
            PathResolver.Resolve(Root, "users.name"));
    }
}
