namespace JsonLinq.Benchmarks.Benchmarks;

/// <summary>
/// Shared inline JSON datasets for benchmarks.
/// Inlined to avoid BenchmarkDotNet child-process file path isolation issues.
/// </summary>
internal static class BenchmarkData
{
    internal const string SmallJson = """
        {
          "users": [
            { "id": 1, "name": "Ava Adams", "age": 30, "department": "Engineering", "salary": 1000, "active": true },
            { "id": 2, "name": "Ben Brown", "age": 25, "department": "Engineering", "salary": 800, "active": true },
            { "id": 3, "name": "Cara Chen", "age": 29, "department": "Sales", "salary": 900, "active": false },
            { "id": 4, "name": "David Davis", "age": 35, "department": "Engineering", "salary": 1200, "active": true },
            { "id": 5, "name": "Eva Evans", "age": 28, "department": "Marketing", "salary": 850, "active": true },
            { "id": 6, "name": "Frank Foster", "age": 32, "department": "Sales", "salary": 950, "active": true },
            { "id": 7, "name": "Grace Green", "age": 27, "department": "Engineering", "salary": 900, "active": false },
            { "id": 8, "name": "Hank Harris", "age": 31, "department": "Marketing", "salary": 880, "active": true },
            { "id": 9, "name": "Iris Islam", "age": 26, "department": "Sales", "salary": 870, "active": true },
            { "id": 10, "name": "Jack Johnson", "age": 33, "department": "Engineering", "salary": 1100, "active": true }
          ]
        }
        """;

    internal const string MediumJson = """
        {
          "users": [
            { "id": 1, "name": "User 1", "age": 30, "department": "Engineering", "salary": 1000, "active": true },
            { "id": 2, "name": "User 2", "age": 25, "department": "Engineering", "salary": 800, "active": true },
            { "id": 3, "name": "User 3", "age": 29, "department": "Sales", "salary": 900, "active": false },
            { "id": 4, "name": "User 4", "age": 35, "department": "Engineering", "salary": 1200, "active": true },
            { "id": 5, "name": "User 5", "age": 28, "department": "Marketing", "salary": 850, "active": true },
            { "id": 6, "name": "User 6", "age": 32, "department": "Sales", "salary": 950, "active": true },
            { "id": 7, "name": "User 7", "age": 27, "department": "Engineering", "salary": 900, "active": false },
            { "id": 8, "name": "User 8", "age": 31, "department": "Marketing", "salary": 880, "active": true },
            { "id": 9, "name": "User 9", "age": 26, "department": "Sales", "salary": 870, "active": true },
            { "id": 10, "name": "User 10", "age": 33, "department": "Engineering", "salary": 1100, "active": true },
            { "id": 11, "name": "User 11", "age": 29, "department": "Engineering", "salary": 950, "active": true },
            { "id": 12, "name": "User 12", "age": 31, "department": "Sales", "salary": 920, "active": true },
            { "id": 13, "name": "User 13", "age": 28, "department": "Marketing", "salary": 860, "active": false },
            { "id": 14, "name": "User 14", "age": 34, "department": "Engineering", "salary": 1150, "active": true },
            { "id": 15, "name": "User 15", "age": 26, "department": "Sales", "salary": 880, "active": true },
            { "id": 16, "name": "User 16", "age": 30, "department": "Marketing", "salary": 870, "active": true },
            { "id": 17, "name": "User 17", "age": 32, "department": "Engineering", "salary": 1080, "active": false },
            { "id": 18, "name": "User 18", "age": 27, "department": "Sales", "salary": 900, "active": true },
            { "id": 19, "name": "User 19", "age": 29, "department": "Marketing", "salary": 845, "active": true },
            { "id": 20, "name": "User 20", "age": 35, "department": "Engineering", "salary": 1180, "active": true }
          ]
        }
        """;
}
