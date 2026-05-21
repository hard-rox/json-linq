namespace JsonLinq.Exceptions;

/// <summary>
/// Represents errors caused by invalid JSON paths.
/// </summary>
public sealed class InvalidPathException : JsonQueryException
{
    /// <summary>
    /// Creates an invalid path exception.
    /// </summary>
    public InvalidPathException(string message)
        : base(message)
    {
    }
}
