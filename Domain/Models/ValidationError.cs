namespace Domain.Models;

public class ValidationError
{
    public string? Source { get; init; }
    public string? Message { get; init; }
    public string? PropertyName { get; init; }
    public object? AttemptedValue { get; init; }
    public string? ErrorCode { get; init; }
}