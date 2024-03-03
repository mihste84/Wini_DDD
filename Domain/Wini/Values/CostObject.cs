namespace Domain.Wini.Values;

public record CostObject
{
    public string? Value { get; }
    public string? Type { get; }
    public readonly int Number;

    public CostObject(int number)
    {
        Number = number;
    }
    public CostObject(int number, string? costObject, string? type)
    {
        Value = costObject;
        Type = type;
        Number = number;

        var validator = new CostObjectValidator(number, false);
        var result = validator.Validate(this);
        if (result.IsValid)
        {
            return;
        }

        throw new DomainValidationException(result.Errors);
    }

    public CostObject Copy() => new(Number, Value, Type);
}