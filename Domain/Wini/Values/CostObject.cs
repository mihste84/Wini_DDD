namespace Domain.Wini.Values;

public readonly record struct CostObject
{
    public readonly string? Value;
    public readonly string? Type;
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
}