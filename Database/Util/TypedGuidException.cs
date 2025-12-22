namespace Database.Util;

public class TypedGuidException : Exception
{
    public TypedGuidException()
    {
    }

    public TypedGuidException(string message)
        : base(message)
    {
    }

    public TypedGuidException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}