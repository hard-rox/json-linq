using JsonLinq.Core;

namespace JsonLinq.Examples.Scenarios;

public static class BasicFiltering
{
    public static void Run(string json)
    {
        var result = JsonQuery.Parse(json)
            .From("users")
            .Where("department", "==", "Engineering")
            .Get();

        Console.WriteLine($"BasicFiltering: {result.Count} engineering users");
    }
}
