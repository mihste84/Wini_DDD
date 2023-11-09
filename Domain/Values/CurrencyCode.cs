namespace Domain.Values;

public record CurrencyCode
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

    public string Code { get; }

    public CurrencyCode(string code)
    {
        if (!string.IsNullOrWhiteSpace(code) && code.Length > 3)
        {
            throw new TextValidationException(
                nameof(code),
                code,
                ValidationErrorCodes.TextTooLong,
                "Currency code cannot be longer than 3 characters"
            )
            { MaxLength = 3 };
        }

        Code = code;
    }

    public CurrencyCode(Country country)
    {
        if (!_currenciesByCountry.TryGetValue(country.Code, out var currency))
        {
            throw new DomainLogicException(
                nameof(country),
                ValidationErrorCodes.IncorrectValue,
                $"Country code value {country.Code} is not allowed"
            );
        }

        Code = currency;
    }
}