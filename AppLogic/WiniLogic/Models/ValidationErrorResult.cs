namespace AppLogic.WiniLogic.Models;

public readonly struct ValidationErrorResult(IEnumerable<ValidationError> value)
{
    public readonly IEnumerable<ValidationError> Value { get; } = value;
}