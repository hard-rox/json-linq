# Getting Started

JsonLinq provides fluent querying over JSON.

## Create a Query

```csharp
using JsonLinq;

var query = JsonQuery.Parse(json).From("users");
```

## Filter and Retrieve

```csharp
var result = query
	.Where(u => u.Value<string>("department") == "Engineering")
	.ToList();
```

## Nested field filtering

```csharp
var sfUsers = query
	.Where(u => u.ValueAt<string>("address.city") == "San Francisco")
	.ToList();
```

## Ordering and aggregation

```csharp
var topSalary = query
	.OrderByDescending("salary")
	.FirstOrDefault();

decimal averageSalary = query.Average("salary");
```
