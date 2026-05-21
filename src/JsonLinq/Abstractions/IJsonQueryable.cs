using JsonLinq.Core;

namespace JsonLinq.Abstractions;

/// <summary>
/// Defines fluent operations for querying JSON data.
/// </summary>
public interface IJsonQueryable
{
    /// <summary>Selects a node and sets it as the current query scope.</summary>
    JsonQuery From(string path);

    /// <summary>Filters current results by a condition.</summary>
    JsonQuery Where(string field, string op, object? value);

    /// <summary>Adds an OR condition against the current scope.</summary>
    JsonQuery OrWhere(string field, string op, object? value);

    /// <summary>Sorts results by a field path in ascending order.</summary>
    JsonQuery OrderBy(string field);

    /// <summary>Sorts results by a field path in descending order.</summary>
    JsonQuery OrderByDescending(string field);

    /// <summary>Groups results by a field path.</summary>
    JsonQuery GroupBy(string field);

    /// <summary>Removes duplicate items by JSON representation.</summary>
    JsonQuery Distinct();

    /// <summary>Breaks results into fixed-size chunks.</summary>
    JsonQuery Chunk(int size);

    /// <summary>Projects each result to a new object with only the specified fields.</summary>
    JsonQuery Select(params string[] fields);

    /// <summary>Returns at most <paramref name="count"/> results from the start.</summary>
    JsonQuery Take(int count);

    /// <summary>Skips the first <paramref name="count"/> results.</summary>
    JsonQuery Skip(int count);

    /// <summary>Returns the number of results.</summary>
    int Count();

    /// <summary>Returns the sum of a numeric field.</summary>
    decimal Sum(string field);

    /// <summary>Returns the average of a numeric field.</summary>
    decimal Average(string field);

    /// <summary>Returns the minimum of a numeric field.</summary>
    decimal Min(string field);

    /// <summary>Returns the maximum of a numeric field.</summary>
    decimal Max(string field);

    /// <summary>Returns the first result, or null if the sequence is empty.</summary>
    JsonNode? FirstOrDefault();

    /// <summary>Returns the last result, or null if the sequence is empty.</summary>
    JsonNode? LastOrDefault();

    /// <summary>Returns the only element, or null if the sequence is empty. Throws <see cref="InvalidOperationException"/> if more than one element exists.</summary>
    JsonNode? SingleOrDefault();

    /// <summary>Returns the result at the given index, or null if out of range.</summary>
    JsonNode? Nth(int index);

    /// <summary>Returns true when the query has at least one result.</summary>
    bool Exists();

    /// <summary>Finds a single node by path without changing scope.</summary>
    JsonNode? Find(string path);

    /// <summary>Alias for <see cref="Find"/>.</summary>
    JsonNode? At(string path);

    /// <summary>Gets query results as a read-only list.</summary>
    IReadOnlyList<JsonNode?> Get();
}
