using System.Text.Json.Nodes;

namespace JsonLinq.Examples.Scenarios;

public static class Sorting
{
    public static void Run(string json)
    {
        JsonNode? first = JsonQuery.Parse(json)
            .From("employees")
            .OrderByDescending("salary")
            .FirstOrDefault();

        Console.WriteLine($"Sorting: top salary employee = {first?["name"]?.GetValue<string>()}");
    }
}
