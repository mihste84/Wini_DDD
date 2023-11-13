namespace Domain.Values;

public record CostObject
{
    public string? Value { get; }
    public string? Type { get; }
    public readonly int Number;

    public CostObject(int number, string? costObject, string? type)
    {
        Value = costObject;
        Type = type;
        Number = number;

        var validator = new CostObjectValidator(false);
        var result = validator.Validate(this);
        if (!result.IsValid)
            throw new DomainValidationException(result.Errors);
    }
}