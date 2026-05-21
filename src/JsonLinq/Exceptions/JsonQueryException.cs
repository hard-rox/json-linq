namespace JsonLinq.Exceptions;

/// <summary>
/// Represents errors that occur while querying JSON.
/// </summary>
public class JsonQueryException : Exception
{
    /// <summary>
    /// Creates an exception with message.
    /// </summary>
    public JsonQueryException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Creates an exception with message and inner exception.
    /// </summary>
    public JsonQueryException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
