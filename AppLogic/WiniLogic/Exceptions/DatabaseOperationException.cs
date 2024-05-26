namespace AppLogic.WiniLogic.Exceptions;

public class DatabaseOperationException : Exception
{
    public DatabaseOperationException() : base()
    {
    }

    public DatabaseOperationException(string? message) : base(message)
    {
    }

    public DatabaseOperationException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}