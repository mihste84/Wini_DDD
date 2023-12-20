namespace Domain.Wini.Values;

public record Subledger
{
    public string? Value { get; }
    public string? Type { get; }

    public Subledger() { }

    public Subledger(string? subledger, string? type)
    {
        Value = subledger;
        Type = type;

        var validator = new SubledgerValidator(false);
        var result = validator.Validate(this);
        if (!result.IsValid)
            throw new DomainValidationException(result.Errors);
    }

    public Subledger Copy() => new(Value, Type);
}