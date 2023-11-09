namespace Domain.Wini.Values;

public record CompanyCode
{
    public int? Code { get; }

    public CompanyCode(string companyCodeString)
    {
        if (string.IsNullOrWhiteSpace(companyCodeString))
            return;

        if (!int.TryParse(companyCodeString.Trim(), out var companyCode))
        {
            throw new TextValidationException(
                nameof(companyCodeString),
                companyCodeString,
                ValidationErrorCodes.IncorrectFormat,
                "CompanyCodeString could not be parsed to a numeric value"
            );
        }

        if (companyCode < 0 || companyCode > 999)
        {
            throw new NumberValidationException(
                nameof(companyCode),
                companyCode,
                ValidationErrorCodes.OutOfRange,
                "CompanyCode must be between 0 and 999"
            )
            { MinValue = 0, MaxValue = 999 };
        }

        Code = companyCode;
    }

    public CompanyCode(int? companyCode)
    {
        if ((companyCode.HasValue && companyCode < 0) || companyCode > 999)
        {
            throw new NumberValidationException(
                nameof(companyCode),
                companyCode.Value,
                ValidationErrorCodes.OutOfRange,
                "CompanyCode must be between 0 and 999"
            )
            { MinValue = 0, MaxValue = 999 };
        }

        Code = companyCode;
    }
}