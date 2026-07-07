# JsonLinq

**JsonLinq** is a simple, elegant .NET library to query over any type of JSON data using a fluent, LINQ-like API. It'll make your life easier by bringing the comfort of LINQ-style querying directly to your JSON.

> Inspired by [php-jsonq](https://github.com/nahid/jsonq) by [Nahid Bin Azhar](https://github.com/nahid).

[![NuGet](https://img.shields.io/nuget/v/JsonLinq.svg)](https://www.nuget.org/packages/JsonLinq)

- Repository: https://github.com/hard-rox/json-linq
- NuGet package: https://www.nuget.org/packages/JsonLinq
- Supported target frameworks: `net8.0`, `net9.0`, `net10.0`

## Installation

```bash
dotnet add package JsonLinq
```

Or in your `.csproj`:

```xml
<PackageReference Include="JsonLinq" Version="*" />
```

## Usage

You can start using this library right away by parsing your JSON data from a string:

```csharp
using JsonLinq;

var q = JsonQuery.Parse(json);
```

From a file:

```csharp
var q = JsonQuery.ParseFile("data.json");
```

Or asynchronously:

```csharp
var q = await JsonQuery.ParseFileAsync("data.json");
```

You can then query your data using methods such as **From**, **Where**, **OrderBy**, **GroupBy**, and more. Aggregate results with **Sum**, **Average**, **Max**, **Min**, and **Count**.

Let's see a quick example. Given this JSON:

```json
// data.json
{
  "employees": [
    { "id": 1, "name": "Alice", "age": 32, "department": "Engineering", "salary": 90000, "active": true,  "address": { "city": "San Francisco", "state": "CA" }, "skills": ["C#", "Python"] },
    { "id": 2, "name": "Bob",   "age": 27, "department": "Engineering", "salary": 78000, "active": false, "address": { "city": "Oakland",       "state": "CA" }, "skills": ["TypeScript", "React"] },
    { "id": 3, "name": "Carol", "age": 30, "department": "Sales",       "salary": 72000, "active": true,  "address": { "city": "Los Angeles",   "state": "CA" }, "skills": ["Salesforce"] }
  ]
}
```

```csharp
using JsonLinq;

var q = JsonQuery.ParseFile("data.json");
var result = q.From("employees")
              .Where(e => e.Value<string>("department") == "Engineering")
              .ToList();
```

This returns all employees in the Engineering department.

Let's say you want the total salary of those employees:

```csharp
decimal total = q.From("employees")
                 .Where(e => e.Value<string>("department") == "Engineering")
                 .Sum("salary");

// 168000
```

Pretty neat, huh?

Let's explore the full API to see what else this library can do for you.

## API

All examples below use the [sample JSON above](#usage).

**List of API:**

- [JsonLinq](#jsonlinq)
  - [Installation](#installation)
  - [Usage](#usage)
  - [API](#api)
    - [`From(path)`](#frompath)
    - [`Find(path)`](#findpath)
    - [`At(path)`](#atpath)
    - [`Where(predicate)`](#wherepredicate)
    - [`OrderBy(field)`](#orderbyfield)
    - [`OrderByDescending(field)`](#orderbydescendingfield)
    - [`GroupBy(field)`](#groupbyfield)
    - [`Chunk(size)`](#chunksize)
    - [`Select(fields)`](#selectfields)
    - [`Distinct()`](#distinct)
    - [`Take(count)`](#takecount)
    - [`Skip(count)`](#skipcount)
    - [`Sum(field)`](#sumfield)
    - [`Average(field)`](#averagefield)
    - [`Max(field)`](#maxfield)
    - [`Min(field)`](#minfield)
    - [`Count()`](#count)
    - [`FirstOrDefault()`](#firstordefault)
    - [`LastOrDefault()`](#lastordefault)
    - [`SingleOrDefault()`](#singleordefault)
    - [`Nth(index)`](#nthindex)
    - [`Exists()`](#exists)
    - [`ToList()`](#tolist)
    - [`Copy()`](#copy)
    - [`Value<T>(key)` / `ValueAt<T>(path)` *(extension methods)*](#valuetkey--valueattpath-extension-methods)
  - [Bugs and Issues](#bugs-and-issues)
  - [Inspired By](#inspired-by)

---

### `From(path)`

* `path` — dot-notated path to a property in the JSON document.

Moves the query scope to the node at the given path. If the target node is a JSON array, each element becomes an item in the working set. Subsequent query methods operate over that set.

**example:**

```csharp
var result = q.From("employees").ToList();
// Returns all 3 employee nodes
```

To navigate deeper:

```csharp
var result = q.From("employees.0.address").ToList();
// Scopes to Alice's address object
```

---

### `Find(path)`

* `path` — dot-notated path to a property.

Returns the raw `JsonNode?` at the given path. Unlike `From`, this terminates the chain and returns the node directly — you cannot chain further query methods after it.

**example:**

```csharp
JsonNode? node = q.Find("employees.0.name");
// JsonValue "Alice"
```

---

### `At(path)`

Alias for `Find(path)`. See [`Find`](#findpath).

---

### `Where(predicate)`

* `predicate` — `Func<JsonNode?, bool>` — a lambda that receives each item and returns `true` to keep it.

Filters the current working set. Use the [`Value<T>(key)`](#valuetkey--valueatpath-extension-methods) and [`ValueAt<T>(path)`](#valuetkey--valueatpath-extension-methods) extension methods inside the predicate to read field values with automatic type conversion.

Multiple `Where` calls are AND-ed together.

**example:**

```csharp
var result = q.From("employees")
              .Where(e => e.Value<string>("department") == "Engineering")
              .ToList();
```

Chaining multiple conditions:

```csharp
var result = q.From("employees")
              .Where(e => e.Value<string>("department") == "Engineering")
              .Where(e => e.Value<int>("age") > 28)
              .ToList();
// Only Alice (age 32, Engineering)
```

For nested fields use `ValueAt<T>`:

```csharp
var result = q.From("employees")
              .Where(e => e.ValueAt<string>("address.city") == "Oakland")
              .ToList();
// Bob
```

---

### `OrderBy(field)`

* `field` — dot-notated path of the field to sort by.

Sorts the working set by `field` in **ascending** order.

**example:**

```csharp
var result = q.From("employees")
              .OrderBy("salary")
              .ToList();
// Carol (72000), Bob (78000), Alice (90000)
```

---

### `OrderByDescending(field)`

* `field` — dot-notated path of the field to sort by.

Sorts the working set by `field` in **descending** order.

**example:**

```csharp
var result = q.From("employees")
              .OrderByDescending("age")
              .ToList();
// Alice (32), Carol (30), Bob (27)
```

---

### `GroupBy(field)`

* `field` — the property to group by.

Groups items by the value of `field`. Returns a new working set where each item is an object with `"key"` and `"items"` properties.

**example:**

```csharp
var result = q.From("employees")
              .GroupBy("department")
              .ToList();
// [
//   { "key": "Engineering", "items": [ Alice, Bob ] },
//   { "key": "Sales",       "items": [ Carol ] }
// ]
```

---

### `Chunk(size)`

* `size` — number of items per chunk. Must be greater than zero.

Splits the working set into consecutive sub-arrays of at most `size` elements.

**example:**

```csharp
var result = q.From("employees")
              .Chunk(2)
              .ToList();
// [ [Alice, Bob], [Carol] ]
```

---

### `Select(fields)`

* `fields` — one or more dot-notated field names to project.

Projects each item to a new object containing only the specified fields. Missing fields are silently omitted.

**example:**

```csharp
var result = q.From("employees")
              .Select("name", "salary")
              .ToList();
// [ { "name": "Alice", "salary": 90000 }, { "name": "Bob", "salary": 78000 }, ... ]
```

---

### `Distinct()`

Removes duplicate items from the working set. Comparison is based on the JSON string representation of each node.

**example:**

```csharp
var result = q.From("employees")
              .Select("department")
              .Distinct()
              .ToList();
// [ { "department": "Engineering" }, { "department": "Sales" } ]
```

---

### `Take(count)`

* `count` — number of items to take from the start. Must be non-negative.

Returns at most `count` items from the beginning of the working set.

**example:**

```csharp
var result = q.From("employees")
              .OrderBy("salary")
              .Take(2)
              .ToList();
// Carol, Bob (two lowest salaries)
```

---

### `Skip(count)`

* `count` — number of items to skip from the start. Must be non-negative.

Skips the first `count` items and returns the rest.

**example:**

```csharp
var result = q.From("employees")
              .OrderByDescending("salary")
              .Skip(1)
              .ToList();
// Bob, Carol (skip the highest earner)
```

---

### `Sum(field)`

* `field` — dot-notated path of the numeric field.

Returns the sum of `field` across all items in the working set.

**example:**

```csharp
decimal total = q.From("employees")
                 .Where(e => e.Value<string>("department") == "Engineering")
                 .Sum("salary");
// 168000
```

---

### `Average(field)`

* `field` — dot-notated path of the numeric field.

Returns the average of `field`. Returns `0` if the working set is empty.

**example:**

```csharp
decimal avg = q.From("employees").Average("salary");
// 80000
```

---

### `Max(field)`

* `field` — dot-notated path of the numeric field.

Returns the maximum value of `field`. Returns `0` if the working set is empty.

**example:**

```csharp
decimal max = q.From("employees").Max("salary");
// 90000
```

---

### `Min(field)`

* `field` — dot-notated path of the numeric field.

Returns the minimum value of `field`. Returns `0` if the working set is empty.

**example:**

```csharp
decimal min = q.From("employees").Min("salary");
// 72000
```

---

### `Count()`

Returns the number of items in the working set.

**example:**

```csharp
int count = q.From("employees")
             .Where(e => e.Value<bool>("active"))
             .Count();
// 2
```

---

### `FirstOrDefault()`

Returns the first item in the working set, or `null` if the set is empty.

**example:**

```csharp
JsonNode? first = q.From("employees")
                   .OrderBy("age")
                   .FirstOrDefault();
// Bob (youngest)
```

---

### `LastOrDefault()`

Returns the last item in the working set, or `null` if the set is empty.

**example:**

```csharp
JsonNode? last = q.From("employees")
                  .OrderBy("age")
                  .LastOrDefault();
// Alice (oldest)
```

---

### `SingleOrDefault()`

Returns the only item in the working set, or `null` if empty. Throws `InvalidOperationException` if the set contains more than one element.

**example:**

```csharp
JsonNode? employee = q.From("employees")
                      .Where(e => e.Value<int>("id") == 1)
                      .SingleOrDefault();
// Alice
```

---

### `Nth(index)`

* `index` — zero-based index of the element to return.

Returns the item at position `index`, or `null` if the index is out of range.

**example:**

```csharp
JsonNode? second = q.From("employees").Nth(1);
// Bob (zero-based index 1)
```

---

### `Exists()`

Returns `true` if the working set contains at least one item, `false` otherwise.

**example:**

```csharp
bool hasEngineers = q.From("employees")
                     .Where(e => e.Value<string>("department") == "Engineering")
                     .Exists();
// true
```

---

### `ToList()`

Returns the current working set as `IReadOnlyList<JsonNode?>`. This is the terminal method for retrieving all results.

**example:**

```csharp
IReadOnlyList<JsonNode?> employees = q.From("employees").ToList();
```

---

### `Copy()`

Returns a deep clone of the current `JsonQuery` instance — including its root document, scope, and working set. Useful when you want to branch a query without affecting the original.

**example:**

```csharp
var original = q.From("employees").Where(e => e.Value<bool>("active"));
var branch   = original.Copy();
// Mutations on branch do not affect original
```

---

### `Value<T>(key)` / `ValueAt<T>(path)` *(extension methods)*

These extension methods on `JsonNode?` live in the `JsonLinq` namespace alongside `JsonQuery`. They are the primary way to access typed field values inside a `Where` predicate (or anywhere you have a `JsonNode?`).

**`Value<T>(key)`** — returns the value of a direct child property cast to `T`, or `default(T)` if the key is missing or the cast fails.

**`ValueAt<T>(path)`** — same as `Value<T>` but accepts a dot-notated path for nested access.

```csharp
using JsonLinq;

// Direct key
string? name = node.Value<string>("name");      // "Alice"
int     age  = node.Value<int>("age");          // 32

// Nested path
string? city = node.ValueAt<string>("address.city"); // "San Francisco"
```

---

## Bugs and Issues

If you encounter any bugs or issues, feel free to [open an issue on GitHub](https://github.com/hard-rox/json-linq/issues).

## Inspired By

**JsonLinq** is inspired by [php-jsonq](https://github.com/nahid/jsonq) — an elegant PHP package for querying JSON data by [Nahid Bin Azhar](https://github.com/nahid). The same concept has been implemented across several languages:

- [php-jsonq](https://github.com/nahid/jsonq) (PHP) by Nahid Bin Azhar
- [js-jsonq](https://github.com/me-shaon/js-jsonq) (JavaScript) by Ahmed Shamim
- [py-jsonq](https://github.com/s1s1ty/py-jsonq) (Python) by Shaon Shaonty
- [gojsonq](https://github.com/thedevsaddam/gojsonq) (Go) by Saddam H
