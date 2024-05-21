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

    public Country(string? code)
    {
        var inputCode = code ?? "";
        if (!_allowedCountries.TryGetValue(inputCode, out var name))
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
        Code = inputCode;

        var validator = new CountryValidator();
        var result = validator.Validate(this);
        if (result.IsValid)
        {
            return;
        }

        throw new DomainValidationException(result.Errors);
    }
}