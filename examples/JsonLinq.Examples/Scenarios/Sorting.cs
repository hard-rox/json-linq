using System.Text.Json.Nodes;
using JsonLinq.Core;

namespace JsonLinq.Examples.Scenarios;

public static class Sorting
{
    public static void Run(string json)
    {
        JsonNode? first = JsonQuery.Parse(json)
            .From("products")
            .OrderByDescending("price")
            .FirstOrDefault();

        Console.WriteLine($"Sorting: top priced product = {first?["name"]?.GetValue<string>()}");
    }
}
