namespace AppLogic.Models;

public readonly struct ValidationErrorResult
{
    public ValidationErrorResult(IEnumerable<ValidationError> value)
    {
        Value = value;
    }

    public readonly IEnumerable<ValidationError> Value { get; }
}