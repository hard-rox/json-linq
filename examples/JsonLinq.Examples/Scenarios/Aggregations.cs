using JsonLinq.Core;

namespace JsonLinq.Examples.Scenarios;

public static class Aggregations
{
    public static void Run(string json)
    {
        var query = JsonQuery.Parse(json).From("users");
        Console.WriteLine($"Aggregations: sum={query.Sum("salary")}, avg={query.Average("salary")}");
    }
}
