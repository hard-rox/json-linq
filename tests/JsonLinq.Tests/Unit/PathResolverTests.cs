using JsonLinq.Exceptions;
using JsonLinq.Utilities;

namespace JsonLinq.Tests.Unit;

public sealed class PathResolverTests
{
    private static readonly JsonNode Root = JsonNode.Parse("""
        { "employees": [ { "name": "Alice" }, { "name": "Bob" } ] }
        """)!;

    [Theory]
    [InlineData("employees.0.name", "Alice")]
    [InlineData("employees.1.name", "Bob")]
    public void Resolve_ValidPath_ReturnsNode(string path, string expected)
    {
        JsonNode? result = PathResolver.Resolve(Root, path);

        Assert.NotNull(result);
        Assert.Equal(expected, result!.GetValue<string>());
    }

    [Fact]
    public void Resolve_EmptyPath_ReturnsRoot()
    {
        JsonNode? result = PathResolver.Resolve(Root, "");
        Assert.Same(Root, result);
    }

    [Fact]
    public void Resolve_MissingKey_ReturnsNull()
    {
        JsonNode? result = PathResolver.Resolve(Root, "nonexistent");
        Assert.Null(result);
    }

    [Fact]
    public void Resolve_ArrayOutOfBounds_ReturnsNull()
    {
        JsonNode? result = PathResolver.Resolve(Root, "employees.99");
        Assert.Null(result);
    }

    [Fact]
    public void Resolve_NullRoot_ReturnsNull()
    {
        JsonNode? result = PathResolver.Resolve(null, "employees");
        Assert.Null(result);
    }

    [Fact]
    public void Resolve_NonNumericArraySegment_ThrowsInvalidPathException()
    {
        Assert.Throws<InvalidPathException>(() =>
            PathResolver.Resolve(Root, "employees.name"));
    }
}
