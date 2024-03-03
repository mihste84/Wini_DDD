namespace Domain.Wini.Values;

public record Account
{
    public string? Value { get; }
    public string? Subsidiary { get; }

    public Account()
    {
    }

    public Account(string? account, string? subsidiary = default)
    {
        Value = account;
        Subsidiary = subsidiary;

        var validator = new AccountValidator(false);
        var result = validator.Validate(this);
        if (result.IsValid)
        {
            return;
        }

        throw new DomainValidationException(result.Errors);
    }

    public Account Copy() => new(Value, Subsidiary);
}
