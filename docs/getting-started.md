# Getting Started

JsonLinq provides fluent querying over JSON.

## Create a Query

```csharp
using JsonLinq.Core;

var query = JsonQuery.Parse(json).From("users");
```

## Filter and Retrieve

```csharp
var result = query.Where("department", "==", "Engineering").Get();
```
