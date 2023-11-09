namespace Domain.Exceptions;

public class NumberValidationException : Exception
{
    public readonly string? PropertyName;
    public readonly double? AttemptedValue;
    public readonly int? ErrorCode;
    public double? MinValue { get; set; }
    public double? MaxValue { get; set; }

    public NumberValidationException(string propertyName, double? value, int errorCode)
        : base("A domin validation error has occured")
    {
        PropertyName = propertyName;
        ErrorCode = errorCode;
        AttemptedValue = value;
    }
    public NumberValidationException(string propertyName, double? value, int errorCode, string message)
        : base(message)
    {
        PropertyName = propertyName;
        ErrorCode = errorCode;
        AttemptedValue = value;
    }

    public NumberValidationException() : base()
    {
    }

    public NumberValidationException(string? message) : base(message)
    {
    }

    public NumberValidationException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}