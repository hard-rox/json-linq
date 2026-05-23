using System.Text.Json.Nodes;

namespace JsonLinq.Examples.Scenarios;

public static class BasicFiltering
{
    public static void Run(string json)
    {
        IReadOnlyList<JsonNode?> result = JsonQuery.Parse(json)
            .From("employees")
            .Where(n => n.Value<string>("department") == "Engineering")
            .ToList();

        Console.WriteLine($"BasicFiltering: {result.Count} engineering employees");
    }
}
