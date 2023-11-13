namespace Domain.Wini.Models;

public class ValidationError
{
    public string? Message { get; init; }
    public string? PropertyName { get; init; }
    public object? AttemptedValue { get; init; }
    public string? ErrorCode { get; init; }
}