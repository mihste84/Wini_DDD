namespace Domain.Exceptions;

public class TextValidationException : Exception
{
    public readonly string? PropertyName;
    public readonly string? AttemptedValue;
    public readonly int? ErrorCode;
    public int? MinLength { get; set; }
    public int? MaxLength { get; set; }

    public TextValidationException(string propertyName, string value, int errorCode)
        : base("A domin validation error has occured")
    {
        PropertyName = propertyName;
        ErrorCode = errorCode;
        AttemptedValue = value;
    }
    public TextValidationException(string propertyName, string value, int errorCode, string message)
        : base(message)
    {
        PropertyName = propertyName;
        ErrorCode = errorCode;
        AttemptedValue = value;
    }

    public TextValidationException() : base()
    {
    }

    public TextValidationException(string? message) : base(message)
    {
    }

    public TextValidationException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}