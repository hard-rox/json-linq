namespace JsonLinq.Tests.Fixtures;

public sealed class SampleDataFixture
{
    public string Json { get; } =
        """
        {
          "employees": [
            { "id": 1, "name": "Alice", "age": 32, "department": "Engineering", "salary": 90000, "active": true,  "address": { "city": "San Francisco", "state": "CA" }, "skills": ["C#", "Python"] },
            { "id": 2, "name": "Bob",   "age": 27, "department": "Engineering", "salary": 78000, "active": false, "address": { "city": "Oakland",       "state": "CA" }, "skills": ["TypeScript", "React"] },
            { "id": 3, "name": "Carol", "age": 30, "department": "Sales",       "salary": 72000, "active": true,  "address": { "city": "Los Angeles",   "state": "CA" }, "skills": ["Salesforce"] }
          ]
        }
        """;
}
