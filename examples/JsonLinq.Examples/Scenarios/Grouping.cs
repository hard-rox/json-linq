using System.Text.Json.Nodes;
using JsonLinq.Core;

namespace JsonLinq.Examples.Scenarios;

public static class Grouping
{
    public static void Run(string json)
    {
        IReadOnlyList<JsonNode?> groups = JsonQuery.Parse(json)
            .From("employees")
            .GroupBy("department")
            .ToList();

        Console.WriteLine($"Grouping: {groups.Count} groups");
    }
}
