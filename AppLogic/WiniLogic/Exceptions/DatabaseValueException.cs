namespace AppLogic.WiniLogic.Exceptions;

public class DatabaseValueException : Exception
{
    public DatabaseValueException() : base()
    {
    }

    public DatabaseValueException(string? message) : base(message)
    {
    }

    public DatabaseValueException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}