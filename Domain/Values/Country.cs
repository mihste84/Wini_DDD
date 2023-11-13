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
        if (!_allowedCountries.TryGetValue(code, out var name))
        {
            throw new DomainValidationException(
                new[] { new ValidationError {
                    AttemptedValue = code,
                    ErrorCode = "IncorrectValue",
                    Message = "Country code value is not allowed.",
                    PropertyName = "Code"
                } }
            );
        }

        Name = name;
        Code = code;

        var validator = new CountryValidator();
        var result = validator.Validate(this);
        if (!result.IsValid)
            throw new DomainValidationException(result.Errors);
    }
}