namespace Domain.Common.Exceptions;

public class DomainLogicException : Exception
{
    public readonly string? PropertyName;
    public readonly string? AttemptedValue;

    public DomainLogicException(string propertyName, string attemptedValue)
        : base("A domin logic error has occured")
    {
        PropertyName = propertyName;
        AttemptedValue = attemptedValue;
    }

    public DomainLogicException(string propertyName, string attemptedValue, string message)
        : base(message)
    {
        PropertyName = propertyName;
        AttemptedValue = attemptedValue;
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