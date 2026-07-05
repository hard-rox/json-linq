using System.Text.Json.Nodes;
using JsonLinq;

string dataPath = Path.Combine(AppContext.BaseDirectory, "data.json");
if (!File.Exists(dataPath))
{
    dataPath = Path.Combine(Directory.GetCurrentDirectory(), "data.json");
}

// ── Creation ────────────────────────────────────────────────────────────────
// Parse from a string.
string json = await File.ReadAllTextAsync(dataPath);
JsonQuery q = JsonQuery.Parse(json);

// ParseFile — load and parse directly from disk.
JsonQuery fromFile = JsonQuery.ParseFile(dataPath);

// ParseFileAsync — asynchronous load and parse.
JsonQuery fromFileAsync = await JsonQuery.ParseFileAsync(dataPath);

Console.WriteLine("== Creation ==");
Console.WriteLine($"Parse            -> {q.From("employees").Count()} employees");
Console.WriteLine($"ParseFile        -> {fromFile.From("employees").Count()} employees");
Console.WriteLine($"ParseFileAsync   -> {fromFileAsync.From("employees").Count()} employees");
Console.WriteLine();

// ── Navigation ───────────────────────────────────────────────────────────────
Console.WriteLine("== Navigation ==");

// From — scope the query to a node (arrays become the working set).
IReadOnlyList<JsonNode?> allEmployees = q.From("employees").ToList();
Console.WriteLine($"From(\"employees\")            -> {allEmployees.Count} items");

// Find — return the raw node at a path (terminates the chain).
JsonNode? aliceName = q.Find("employees.0.name");
Console.WriteLine($"Find(\"employees.0.name\")     -> {aliceName}");

// At — alias for Find.
JsonNode? aliceCity = q.At("employees.0.address.city");
Console.WriteLine($"At(\"employees.0.address.city\") -> {aliceCity}");
Console.WriteLine();

// ── Filtering ────────────────────────────────────────────────────────────────
Console.WriteLine("== Filtering ==");

// Where — predicate using the Value<T> extension.
IReadOnlyList<JsonNode?> engineers = q.From("employees")
    .Where(e => e.Value<string>("department") == "Engineering")
    .ToList();
Console.WriteLine($"Where(department == Engineering) -> {engineers.Count} employees");

// Chained Where — conditions are AND-ed together.
IReadOnlyList<JsonNode?> seniorEngineers = q.From("employees")
    .Where(e => e.Value<string>("department") == "Engineering")
    .Where(e => e.Value<int>("age") > 30)
    .ToList();
Console.WriteLine($"Where(...).Where(age > 30)       -> {seniorEngineers.Count} employees");

// Where with a nested field via ValueAt<T>.
IReadOnlyList<JsonNode?> oaklanders = q.From("employees")
    .Where(e => e.ValueAt<string>("address.city") == "Oakland")
    .ToList();
Console.WriteLine($"Where(address.city == Oakland)   -> {oaklanders.Count} employees");
Console.WriteLine();

// ── Ordering ─────────────────────────────────────────────────────────────────
Console.WriteLine("== Ordering ==");

// OrderBy — ascending.
IReadOnlyList<JsonNode?> bySalary = q.From("employees").OrderBy("salary").ToList();
Console.WriteLine($"OrderBy(salary)           -> lowest: {bySalary[0].Value<string>("name")}");

// OrderByDescending — descending.
IReadOnlyList<JsonNode?> byAgeDesc = q.From("employees").OrderByDescending("age").ToList();
Console.WriteLine($"OrderByDescending(age)    -> oldest: {byAgeDesc[0].Value<string>("name")}");
Console.WriteLine();

// ── Grouping & Chunking ───────────────────────────────────────────────────────
Console.WriteLine("== Grouping & Chunking ==");

// GroupBy — groups into { key, items } objects.
IReadOnlyList<JsonNode?> byDepartment = q.From("employees").GroupBy("department").ToList();
Console.WriteLine($"GroupBy(department)       -> {byDepartment.Count} groups");
foreach (JsonNode? group in byDepartment)
{
    string? key = group.Value<string>("key");
    int count = group?["items"]?.AsArray().Count ?? 0;
    Console.WriteLine($"    {key}: {count}");
}

// Chunk — split into sub-arrays of a fixed size.
IReadOnlyList<JsonNode?> chunks = q.From("employees").Chunk(2).ToList();
Console.WriteLine($"Chunk(2)                  -> {chunks.Count} chunks");
Console.WriteLine();

// ── Projection ───────────────────────────────────────────────────────────────
Console.WriteLine("== Projection ==");

// Select — project each item to a subset of fields.
IReadOnlyList<JsonNode?> projected = q.From("employees").Select("name", "salary").ToList();
Console.WriteLine($"Select(name, salary)      -> {projected[0]}");

// Distinct — remove duplicate nodes.
IReadOnlyList<JsonNode?> distinctDepartments = q.From("employees")
    .Select("department")
    .Distinct()
    .ToList();
Console.WriteLine($"Select(department).Distinct() -> {distinctDepartments.Count} unique");
Console.WriteLine();

// ── Pagination ───────────────────────────────────────────────────────────────
Console.WriteLine("== Pagination ==");

// Take — first N items.
IReadOnlyList<JsonNode?> cheapestTwo = q.From("employees").OrderBy("salary").Take(2).ToList();
Console.WriteLine($"OrderBy(salary).Take(2)   -> {cheapestTwo.Count} items");

// Skip — drop the first N items.
IReadOnlyList<JsonNode?> afterTop = q.From("employees").OrderByDescending("salary").Skip(1).ToList();
Console.WriteLine($"OrderByDescending(salary).Skip(1) -> {afterTop.Count} items");
Console.WriteLine();

// ── Aggregation ──────────────────────────────────────────────────────────────
Console.WriteLine("== Aggregation ==");
Console.WriteLine($"Sum(salary)     -> {q.From("employees").Sum("salary")}");
Console.WriteLine($"Average(salary) -> {q.From("employees").Average("salary")}");
Console.WriteLine($"Max(salary)     -> {q.From("employees").Max("salary")}");
Console.WriteLine($"Min(salary)     -> {q.From("employees").Min("salary")}");
Console.WriteLine($"Count()         -> {q.From("employees").Count()}");
Console.WriteLine();

// ── Element access ───────────────────────────────────────────────────────────
Console.WriteLine("== Element access ==");

// FirstOrDefault / LastOrDefault.
JsonNode? youngest = q.From("employees").OrderBy("age").FirstOrDefault();
JsonNode? oldest = q.From("employees").OrderBy("age").LastOrDefault();
Console.WriteLine($"FirstOrDefault() (by age) -> {youngest.Value<string>("name")}");
Console.WriteLine($"LastOrDefault()  (by age) -> {oldest.Value<string>("name")}");

// SingleOrDefault — exactly one match expected.
JsonNode? alice = q.From("employees")
    .Where(e => e.Value<int>("id") == 1)
    .SingleOrDefault();
Console.WriteLine($"SingleOrDefault() (id == 1) -> {alice.Value<string>("name")}");

// Nth — element at a zero-based index.
JsonNode? second = q.From("employees").Nth(1);
Console.WriteLine($"Nth(1)                    -> {second.Value<string>("name")}");

// Exists — is the working set non-empty?
bool hasEngineers = q.From("employees")
    .Where(e => e.Value<string>("department") == "Engineering")
    .Exists();
Console.WriteLine($"Exists() (engineers)      -> {hasEngineers}");
Console.WriteLine();

// ── Terminal & utility ────────────────────────────────────────────────────────
Console.WriteLine("== Terminal & utility ==");

// ToList — materialize the working set.
IReadOnlyList<JsonNode?> materialized = q.From("employees").ToList();
Console.WriteLine($"ToList()                  -> {materialized.Count} items");

// Copy — deep clone a query to branch without side effects.
JsonQuery original = q.From("employees").Where(e => e.Value<bool>("active"));
JsonQuery branch = original.Copy();
Console.WriteLine($"Copy() (active employees) -> original: {original.Count()}, branch: {branch.Count()}");
