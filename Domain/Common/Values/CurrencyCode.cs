namespace Domain.Common.Values;

public readonly record struct CurrencyCode
{
    private readonly Dictionary<string, string> _currenciesByCountry = new() {
        { "SE", "SEK" },
        { "NO", "NOK" },
        { "FI", "EUR" },
        { "DK", "DKK" },
        { "LT", "EUR" },
        { "LV", "EUR" },
        { "EE", "EUR" }
    };

    public readonly string? Code;

    public CurrencyCode()
    {
    }

    public CurrencyCode(string? code)
    {
        Code = code;

        var validator = new CurrencyCodeValidator(false);
        var result = validator.Validate(this);
        if (result.IsValid)
        {
            return;
        }

        throw new DomainValidationException(result.Errors);
    }

    public CurrencyCode(Country country)
    {
        if (!_currenciesByCountry.TryGetValue(country.Code, out var currency))
        {
            throw new DomainValidationException(
                new[] { new ValidationError {
                    AttemptedValue = country.Code,
                    ErrorCode = "IncorrectValue",
                    Message = "Country code value is not allowed.",
                    PropertyName = "Code"
                } }
            );
        }

        Code = currency;

        var validator = new CurrencyCodeValidator(false);
        var result = validator.Validate(this);
        if (result.IsValid)
        {
            return;
        }

        throw new DomainValidationException(result.Errors);
    }
}