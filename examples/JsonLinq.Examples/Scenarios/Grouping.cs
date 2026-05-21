using JsonLinq.Core;

namespace JsonLinq.Examples.Scenarios;

public static class Grouping
{
    public static void Run(string json)
    {
        var groups = JsonQuery.Parse(json)
            .From("users")
            .GroupBy("department")
            .Get();

        Console.WriteLine($"Grouping: {groups.Count} groups");
    }
}
