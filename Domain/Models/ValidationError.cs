namespace Domain.Models;

public record ValidationError
{
    public string? Message { get; init; }
    public string? PropertyName { get; init; }
    public object? AttemptedValue { get; init; }
    public string? ErrorCode { get; init; }

    public ValidationError()
    {
    }
    public ValidationError(ValidationFailure failure)
    {
        Message = failure.ErrorMessage;
        PropertyName = failure.PropertyName;
        AttemptedValue = failure.AttemptedValue;
        ErrorCode = failure.ErrorCode;
    }
}