using JsonLinq.Tests.Fixtures;

namespace JsonLinq.Tests.Unit;

public sealed class JsonQueryTests
{
    private readonly SampleDataFixture _fixture = new();

    // ── Basic navigation ───────────────────────────────────────────────────────

    [Fact]
    public void FromAndFirstOrDefault_ReturnsFirstUser()
    {
        JsonNode? first = JsonQuery.Parse(_fixture.Json).From("employees").FirstOrDefault();

        Assert.NotNull(first);
        Assert.Equal("Alice", JsonTestHelper.GetString(first, "name"));
    }

    [Fact]
    public void LastOrDefault_ReturnsLastUser()
    {
        JsonNode? last = JsonQuery.Parse(_fixture.Json).From("employees").LastOrDefault();

        Assert.NotNull(last);
        Assert.Equal("Carol", JsonTestHelper.GetString(last, "name"));
    }

    [Fact]
    public void Nth_ValidIndex_ReturnsCorrectNode()
    {
        JsonNode? node = JsonQuery.Parse(_fixture.Json).From("employees").Nth(1);

        Assert.NotNull(node);
        Assert.Equal("Bob", JsonTestHelper.GetString(node, "name"));
    }

    [Fact]
    public void Nth_NegativeIndex_ReturnsNull()
    {
        JsonNode? node = JsonQuery.Parse(_fixture.Json).From("employees").Nth(-1);
        Assert.Null(node);
    }

    [Fact]
    public void Nth_OutOfBoundsIndex_ReturnsNull()
    {
        JsonNode? node = JsonQuery.Parse(_fixture.Json).From("employees").Nth(999);
        Assert.Null(node);
    }

    [Fact]
    public void Find_ReturnsNodeAtPath()
    {
        JsonNode? node = JsonQuery.Parse(_fixture.Json).Find("employees.0.name");
        Assert.Equal("Alice", node?.GetValue<string>());
    }

    [Fact]
    public void At_IsAliasForFind()
    {
        JsonQuery q = JsonQuery.Parse(_fixture.Json);
        Assert.Equal(q.Find("employees.0.name")?.ToJsonString(), q.At("employees.0.name")?.ToJsonString());
    }

    // ── Filtering ─────────────────────────────────────────────────────────────

    [Fact]
    public void Where_Predicate_FiltersCorrectly()
    {
        IReadOnlyList<JsonNode?> result = JsonQuery.Parse(_fixture.Json)
            .From("employees")
            .Where(n => n.Value<string>("department") == "Engineering")
            .ToList();

        Assert.Equal(2, result.Count);
        Assert.All(result, n => Assert.Equal("Engineering", JsonTestHelper.GetString(n, "department")));
    }

    [Fact]
    public void Where_Predicate_NestedPath_FiltersCorrectly()
    {
        IReadOnlyList<JsonNode?> result = JsonQuery.Parse(_fixture.Json)
            .From("employees")
            .Where(n => n.ValueAt<string>("address.city") == "Oakland")
            .ToList();

        Assert.Single(result);
        Assert.Equal("Bob", JsonTestHelper.GetString(result[0], "name"));
    }

    [Fact]
    public void Where_Predicate_MissingField_ReturnsEmpty()
    {
        int count = JsonQuery.Parse(_fixture.Json)
            .From("employees")
            .Where(n => n.Value<string>("nonexistent") == "x")
            .Count();

        Assert.Equal(0, count);
    }

    [Fact]
    public void Where_OrPredicate_CombinesResults()
    {
        int count = JsonQuery.Parse(_fixture.Json)
            .From("employees")
            .Where(n => n.Value<string>("name") == "Alice" || n.Value<string>("name") == "Bob")
            .Count();

        Assert.Equal(2, count);
    }

    // ── Sorting ───────────────────────────────────────────────────────────────

    [Fact]
    public void OrderByDescending_ReturnsHighestAgeFirst()
    {
        JsonNode? first = JsonQuery.Parse(_fixture.Json).From("employees").OrderByDescending("age").FirstOrDefault();
        Assert.Equal("Alice", JsonTestHelper.GetString(first, "name"));
    }

    // ── Aggregations ──────────────────────────────────────────────────────────

    [Fact]
    public void Min_ReturnsLowestSalary()
    {
        decimal min = JsonQuery.Parse(_fixture.Json).From("employees").Min("salary");
        Assert.Equal(72000M, min);
    }

    [Fact]
    public void Max_ReturnsHighestSalary()
    {
        decimal max = JsonQuery.Parse(_fixture.Json).From("employees").Max("salary");
        Assert.Equal(90000M, max);
    }

    // ── Transformations ───────────────────────────────────────────────────────

    [Fact]
    public void Chunk_ProducesCorrectNumberOfChunks()
    {
        // 3 employees chunked by 2 → 2 chunks
        IReadOnlyList<JsonNode?> chunks = JsonQuery.Parse(_fixture.Json).From("employees").Chunk(2).ToList();
        Assert.Equal(2, chunks.Count);
    }

    [Fact]
    public void Chunk_FirstChunkHasCorrectSize()
    {
        JsonArray? first = JsonQuery.Parse(_fixture.Json).From("employees").Chunk(2).FirstOrDefault() as JsonArray;
        Assert.NotNull(first);
        Assert.Equal(2, first!.Count);
    }

    [Fact]
    public void Chunk_ZeroSize_ThrowsArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            JsonQuery.Parse(_fixture.Json).From("employees").Chunk(0));
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
        JsonQuery original = JsonQuery.Parse(_fixture.Json).From("employees");
        JsonQuery copy = original.Copy();

        // Same count
        Assert.Equal(original.Count(), copy.Count());
        // Same content
        Assert.Equal(original.ToList()[0]?.ToJsonString(), copy.ToList()[0]?.ToJsonString());
    }

    // ── Exists ────────────────────────────────────────────────────────────────

    [Fact]
    public void Exists_ReturnsTrueWhenResultsPresent()
    {
        Assert.True(JsonQuery.Parse(_fixture.Json).From("employees").Exists());
    }

    [Fact]
    public void Exists_ReturnsFalseWhenNoMatches()
    {
        bool exists = JsonQuery.Parse(_fixture.Json)
            .From("employees")
            .Where(n => n.Value<string>("name") == "DoesNotExist")
            .Exists();
        Assert.False(exists);
    }

    // ── Take / Skip ───────────────────────────────────────────────────────────

    [Fact]
    public void Take_ReturnsAtMostNResults()
    {
        IReadOnlyList<JsonNode?> result = JsonQuery.Parse(_fixture.Json).From("employees").Take(2).ToList();
        Assert.Equal(2, result.Count);
        Assert.Equal("Alice", JsonTestHelper.GetString(result[0], "name"));
    }

    [Fact]
    public void Skip_SkipsFirstNResults()
    {
        IReadOnlyList<JsonNode?> result = JsonQuery.Parse(_fixture.Json).From("employees").Skip(1).ToList();
        Assert.Equal(2, result.Count);
        Assert.Equal("Bob", JsonTestHelper.GetString(result[0], "name"));
    }

    [Fact]
    public void Skip_Then_Take_ReturnsMiddleItem()
    {
        IReadOnlyList<JsonNode?> result = JsonQuery.Parse(_fixture.Json).From("employees").Skip(1).Take(1).ToList();
        Assert.Single(result);
        Assert.Equal("Bob", JsonTestHelper.GetString(result[0], "name"));
    }

    [Fact]
    public void Take_NegativeCount_ThrowsArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            JsonQuery.Parse(_fixture.Json).From("employees").Take(-1));
    }

    [Fact]
    public void Skip_NegativeCount_ThrowsArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            JsonQuery.Parse(_fixture.Json).From("employees").Skip(-1));
    }

    // ── SingleOrDefault ───────────────────────────────────────────────────────

    [Fact]
    public void SingleOrDefault_EmptySequence_ReturnsNull()
    {
        JsonNode? result = JsonQuery.Parse(_fixture.Json)
            .From("employees")
            .Where(n => n.Value<string>("name") == "DoesNotExist")
            .SingleOrDefault();

        Assert.Null(result);
    }

    [Fact]
    public void SingleOrDefault_ExactlyOneElement_ReturnsElement()
    {
        JsonNode? result = JsonQuery.Parse(_fixture.Json)
            .From("employees")
            .Where(n => n.Value<string>("name") == "Alice")
            .SingleOrDefault();

        Assert.NotNull(result);
        Assert.Equal("Alice", JsonTestHelper.GetString(result, "name"));
    }

    [Fact]
    public void SingleOrDefault_MultipleElements_ThrowsInvalidOperationException()
    {
        Assert.Throws<InvalidOperationException>(() =>
            JsonQuery.Parse(_fixture.Json).From("employees").SingleOrDefault());
    }

    // ── Select / Projection ───────────────────────────────────────────────────

    [Fact]
    public void Select_ReturnsOnlyRequestedFields()
    {
        IReadOnlyList<JsonNode?> results = JsonQuery.Parse(_fixture.Json)
            .From("employees")
            .Select("name", "age")
            .ToList();

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
            .From("employees")
            .Select("name", "nonexistent")
            .ToList();

        JsonObject? first = results[0] as JsonObject;
        Assert.NotNull(first);
        Assert.NotNull(first!["name"]);
        Assert.Null(first["nonexistent"]);
    }

    // ── Edge cases ────────────────────────────────────────────────────────────

    [Fact]
    public void EmptyArray_Count_ReturnsZeroNotThrow()
    {
        int count = JsonQuery.Parse("{\"employees\":[]}").From("employees").Count();
        Assert.Equal(0, count);
    }
}
