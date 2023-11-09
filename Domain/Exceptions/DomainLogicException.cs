namespace Domain.Exceptions;

public class DomainLogicException : Exception
{
    public readonly string? PropertyName;
    public readonly int? ErrorCode;

    public DomainLogicException(string propertyName, int errorCode)
        : base("A domin logic error has occured")
    {
        PropertyName = propertyName;
        ErrorCode = errorCode;
    }
    public DomainLogicException(string propertyName, int errorCode, string message)
        : base(message)
    {
        PropertyName = propertyName;
        ErrorCode = errorCode;
    }

    public DomainLogicException() : base()
    {
    }

    public DomainLogicException(string? message) : base(message)
    {
    }

    public DomainLogicException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}