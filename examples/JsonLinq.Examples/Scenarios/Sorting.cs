using System.Text.Json.Nodes;
using JsonLinq.Core;

namespace JsonLinq.Examples.Scenarios;

public static class Sorting
{
    public static void Run(string json)
    {
        JsonNode? first = JsonQuery.Parse(json)
            .From("users")
            .OrderByDescending("salary")
            .FirstOrDefault();

        Console.WriteLine($"Sorting: top salary user = {first?["name"]?.GetValue<string>()}");
    }
}
