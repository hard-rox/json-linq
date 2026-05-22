using System.Text.Json.Nodes;
using JsonLinq.Core;

namespace JsonLinq.Examples.Scenarios;

public static class BasicFiltering
{
    public static void Run(string json)
    {
        IReadOnlyList<JsonNode?> result = JsonQuery.Parse(json)
            .From("employees")
            .Where("department", "==", "Engineering")
            .Get();

        Console.WriteLine($"BasicFiltering: {result.Count} engineering employees");
    }
}
