namespace Domain.Common.Values;

public readonly record struct Subledger
{
    public readonly string? Value;
    public readonly string? Type;

    public Subledger()
    {
    }

    public Subledger(string? subledger, string? type)
    {
        Value = subledger;
        Type = type;

        var validator = new SubledgerValidator(false);
        var result = validator.Validate(this);
        if (result.IsValid)
        {
            return;
        }

        throw new DomainValidationException(result.Errors);
    }

    public Subledger Copy() => new(Value, Type);
}