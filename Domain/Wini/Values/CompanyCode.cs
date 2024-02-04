namespace Domain.Wini.Values;

public record CompanyCode
{
    public int? Code { get; }

    public CompanyCode() { }

    public CompanyCode(string? companyCodeString)
    {
        if (string.IsNullOrWhiteSpace(companyCodeString))
            return;

        if (!int.TryParse(companyCodeString.Trim(), out var companyCode))
        {
            throw new FormatException(
                "CompanyCodeString could not be parsed to a numeric value"
            );
        }

        Code = companyCode;

        var validator = new CompanyCodeValidator(false);
        var result = validator.Validate(this);
        if (!result.IsValid)
            throw new DomainValidationException(result.Errors);
    }

    public CompanyCode(int? companyCode)
    {
        Code = companyCode;

        if (Code.HasValue)
        {
            var validator = new CompanyCodeValidator(false);
            var result = validator.Validate(this);
            if (!result.IsValid)
                throw new DomainValidationException(result.Errors);
        }
    }
}