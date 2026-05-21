using JsonLinq.Core;
using JsonLinq.Tests.Fixtures;

namespace JsonLinq.Tests.Unit;

public sealed class JsonQueryTests
{
    private readonly SampleDataFixture _fixture = new();

    // ── Basic navigation ───────────────────────────────────────────────────────

    [Fact]
    public void FromAndFirstOrDefault_ReturnsFirstUser()
    {
        JsonNode? first = JsonQuery.Parse(_fixture.Json).From("users").FirstOrDefault();

        Assert.NotNull(first);
        Assert.Equal("Ava", JsonTestHelper.GetString(first, "name"));
    }

    [Fact]
    public void LastOrDefault_ReturnsLastUser()
    {
        JsonNode? last = JsonQuery.Parse(_fixture.Json).From("users").LastOrDefault();

        Assert.NotNull(last);
        Assert.Equal("Cara", JsonTestHelper.GetString(last, "name"));
    }

    [Fact]
    public void Nth_ValidIndex_ReturnsCorrectNode()
    {
        JsonNode? node = JsonQuery.Parse(_fixture.Json).From("users").Nth(1);

        Assert.NotNull(node);
        Assert.Equal("Ben", JsonTestHelper.GetString(node, "name"));
    }

    [Fact]
    public void Nth_NegativeIndex_ReturnsNull()
    {
        JsonNode? node = JsonQuery.Parse(_fixture.Json).From("users").Nth(-1);
        Assert.Null(node);
    }

    [Fact]
    public void Nth_OutOfBoundsIndex_ReturnsNull()
    {
        JsonNode? node = JsonQuery.Parse(_fixture.Json).From("users").Nth(999);
        Assert.Null(node);
    }

    [Fact]
    public void Find_ReturnsNodeAtPath()
    {
        JsonNode? node = JsonQuery.Parse(_fixture.Json).Find("users.0.name");
        Assert.Equal("Ava", node?.GetValue<string>());
    }

    [Fact]
    public void At_IsAliasForFind()
    {
        JsonQuery q = JsonQuery.Parse(_fixture.Json);
        Assert.Equal(q.Find("users.0.name")?.ToJsonString(), q.At("users.0.name")?.ToJsonString());
    }

    // ── Filtering ─────────────────────────────────────────────────────────────

    [Fact]
    public void OrWhere_CombinesFilteredResults()
    {
        int count = JsonQuery.Parse(_fixture.Json)
            .From("users")
            .Where("name", "==", "Ava")
            .OrWhere("name", "==", "Ben")
            .Count();

        Assert.Equal(2, count);
    }

    // ── Sorting ───────────────────────────────────────────────────────────────

    [Fact]
    public void OrderByDescending_ReturnsHighestAgeFirst()
    {
        JsonNode? first = JsonQuery.Parse(_fixture.Json).From("users").OrderByDescending("age").FirstOrDefault();
        Assert.Equal("Ava", JsonTestHelper.GetString(first, "name"));
    }

    // ── Aggregations ──────────────────────────────────────────────────────────

    [Fact]
    public void Min_ReturnsLowestSalary()
    {
        decimal min = JsonQuery.Parse(_fixture.Json).From("users").Min("salary");
        Assert.Equal(800M, min);
    }

    [Fact]
    public void Max_ReturnsHighestSalary()
    {
        decimal max = JsonQuery.Parse(_fixture.Json).From("users").Max("salary");
        Assert.Equal(1000M, max);
    }

    // ── Transformations ───────────────────────────────────────────────────────

    [Fact]
    public void Chunk_ProducesCorrectNumberOfChunks()
    {
        // 3 users chunked by 2 → 2 chunks
        IReadOnlyList<JsonNode?> chunks = JsonQuery.Parse(_fixture.Json).From("users").Chunk(2).Get();
        Assert.Equal(2, chunks.Count);
    }

    [Fact]
    public void Chunk_FirstChunkHasCorrectSize()
    {
        JsonArray? first = JsonQuery.Parse(_fixture.Json).From("users").Chunk(2).FirstOrDefault() as JsonArray;
        Assert.NotNull(first);
        Assert.Equal(2, first!.Count);
    }

    [Fact]
    public void Chunk_ZeroSize_ThrowsArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            JsonQuery.Parse(_fixture.Json).From("users").Chunk(0));
    }

    [Fact]
    public void Distinct_RemovesDuplicates()
    {
        // Add a duplicate entry to the JSON
        const string json = """
            {"items": ["a", "b", "a", "c"]}
            """;
        int count = JsonQuery.Parse(json).From("items").Distinct().Count();
        Assert.Equal(3, count);
    }

    [Fact]
    public void Copy_ReturnsEqualButIndependentQuery()
    {
        JsonQuery original = JsonQuery.Parse(_fixture.Json).From("users");
        JsonQuery copy = original.Copy();

        // Same count
        Assert.Equal(original.Count(), copy.Count());
        // Same content
        Assert.Equal(original.Get()[0]?.ToJsonString(), copy.Get()[0]?.ToJsonString());
    }

    // ── Exists ────────────────────────────────────────────────────────────────

    [Fact]
    public void Exists_ReturnsTrueWhenResultsPresent()
    {
        Assert.True(JsonQuery.Parse(_fixture.Json).From("users").Exists());
    }

    [Fact]
    public void Exists_ReturnsFalseWhenNoMatches()
    {
        bool exists = JsonQuery.Parse(_fixture.Json)
            .From("users")
            .Where("name", "==", "DoesNotExist")
            .Exists();
        Assert.False(exists);
    }

    // ── Take / Skip ───────────────────────────────────────────────────────────

    [Fact]
    public void Take_ReturnsAtMostNResults()
    {
        IReadOnlyList<JsonNode?> result = JsonQuery.Parse(_fixture.Json).From("users").Take(2).Get();
        Assert.Equal(2, result.Count);
        Assert.Equal("Ava", JsonTestHelper.GetString(result[0], "name"));
    }

    [Fact]
    public void Skip_SkipsFirstNResults()
    {
        IReadOnlyList<JsonNode?> result = JsonQuery.Parse(_fixture.Json).From("users").Skip(1).Get();
        Assert.Equal(2, result.Count);
        Assert.Equal("Ben", JsonTestHelper.GetString(result[0], "name"));
    }

    [Fact]
    public void Skip_Then_Take_ReturnsMiddleItem()
    {
        IReadOnlyList<JsonNode?> result = JsonQuery.Parse(_fixture.Json).From("users").Skip(1).Take(1).Get();
        Assert.Single(result);
        Assert.Equal("Ben", JsonTestHelper.GetString(result[0], "name"));
    }

    [Fact]
    public void Take_NegativeCount_ThrowsArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            JsonQuery.Parse(_fixture.Json).From("users").Take(-1));
    }

    [Fact]
    public void Skip_NegativeCount_ThrowsArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            JsonQuery.Parse(_fixture.Json).From("users").Skip(-1));
    }

    // ── SingleOrDefault ───────────────────────────────────────────────────────

    [Fact]
    public void SingleOrDefault_EmptySequence_ReturnsNull()
    {
        JsonNode? result = JsonQuery.Parse(_fixture.Json)
            .From("users")
            .Where("name", "==", "DoesNotExist")
            .SingleOrDefault();

        Assert.Null(result);
    }

    [Fact]
    public void SingleOrDefault_ExactlyOneElement_ReturnsElement()
    {
        JsonNode? result = JsonQuery.Parse(_fixture.Json)
            .From("users")
            .Where("name", "==", "Ava")
            .SingleOrDefault();

        Assert.NotNull(result);
        Assert.Equal("Ava", JsonTestHelper.GetString(result, "name"));
    }

    [Fact]
    public void SingleOrDefault_MultipleElements_ThrowsInvalidOperationException()
    {
        Assert.Throws<InvalidOperationException>(() =>
            JsonQuery.Parse(_fixture.Json).From("users").SingleOrDefault());
    }

    // ── Select / Projection ───────────────────────────────────────────────────

    [Fact]
    public void Select_ReturnsOnlyRequestedFields()
    {
        IReadOnlyList<JsonNode?> results = JsonQuery.Parse(_fixture.Json)
            .From("users")
            .Select("name", "age")
            .Get();

        Assert.Equal(3, results.Count);
        JsonObject? first = results[0] as JsonObject;
        Assert.NotNull(first);
        Assert.NotNull(first!["name"]);
        Assert.NotNull(first["age"]);
        Assert.Null(first["salary"]); // not selected
        Assert.Null(first["department"]);
    }

    [Fact]
    public void Select_MissingFieldOmittedSilently()
    {
        IReadOnlyList<JsonNode?> results = JsonQuery.Parse(_fixture.Json)
            .From("users")
            .Select("name", "nonexistent")
            .Get();

        JsonObject? first = results[0] as JsonObject;
        Assert.NotNull(first);
        Assert.NotNull(first!["name"]);
        Assert.Null(first["nonexistent"]);
    }

    // ── Edge cases ────────────────────────────────────────────────────────────

    [Fact]
    public void EmptyArray_Count_ReturnsZeroNotThrow()
    {
        int count = JsonQuery.Parse("{\"users\":[]}").From("users").Count();
        Assert.Equal(0, count);
    }

    [Fact]
    public void Where_MissingField_SilentlyNonMatches()
    {
        int count = JsonQuery.Parse(_fixture.Json)
            .From("users")
            .Where("nonexistent", "==", "x")
            .Count();
        Assert.Equal(0, count);
    }
}
