using Domain.Wini.Validators;

namespace Domain.Wini.Values;
public record Account
{
    public string? Value { get; }
    public string? Subsidiary { get; }

    public Account(string? account, string? subsidiary = default)
    {
        if (!string.IsNullOrWhiteSpace(account) && account.Length > 6)
        {
            throw new TextValidationException(
                nameof(account),
                account,
                ValidationErrorCodes.TextTooLong,
                "Account cannot be longer than 6 characters"
            )
            { MaxLength = 6 };
        }

        if (!string.IsNullOrWhiteSpace(subsidiary) && subsidiary.Length > 8)
        {
            throw new TextValidationException(
                nameof(subsidiary),
                subsidiary,
                ValidationErrorCodes.TextTooLong,
                "Subsidiary cannot be longer than 8 characters"
            )
            { MaxLength = 8 };
        }

        Value = account;
        Subsidiary = subsidiary;
    }

    public async Task<(bool IsValid, IEnumerable<ValidationError>? Errors)> Validate()
    {
        var validator = new AccountValidator();
        var res = await validator.ValidateAsync(this);
        var errors = res.Errors.Select(_ => new ValidationError
        {
            AttemptedValue = _.AttemptedValue,
            ErrorCode = _.ErrorCode,
            Message = _.ErrorMessage,
            PropertyName = _.PropertyName,
            Source = nameof(Account)
        });
        return (res.IsValid, Errors: errors);
    }
}
