namespace Domain.Values;

public record Country
{
    private readonly Dictionary<string, string> _allowedCountries = new() {
        { "SE", "Sweden" },
        { "NO", "Norway" },
        { "FI", "Finland" },
        { "DK", "Denmark" },
        { "LT", "Lithuania" },
        { "LV", "Latvia" },
        { "EE", "Estonia" }
    };

    public readonly string Name;
    public readonly string Code;

    public Country(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            throw new TextValidationException(
                nameof(code),
                code,
                ValidationErrorCodes.Required,
                "Country code is required"
            );
        }

        if (!_allowedCountries.TryGetValue(code, out var name))
        {
            throw new DomainLogicException(
                nameof(code),
                ValidationErrorCodes.IncorrectValue,
                $"Country code value {code} is not allowed"
            );
        }

        Name = name;
        Code = code;
    }
}