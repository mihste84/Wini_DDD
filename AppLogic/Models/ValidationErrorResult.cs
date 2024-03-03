namespace AppLogic.Models;

public readonly struct ValidationErrorResult(IEnumerable<ValidationError> value)
{
    public readonly IEnumerable<ValidationError> Value { get; } = value;
}