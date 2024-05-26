namespace Domain.Common.Values;

public readonly record struct Account
{
    public readonly string? Value;
    public readonly string? Subsidiary;

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
}
