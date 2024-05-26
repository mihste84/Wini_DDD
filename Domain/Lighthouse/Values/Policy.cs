
namespace Domain.Lighthouse.Values;

public readonly record struct Policy
{
    public readonly string? PolicyNumber;
    public readonly string? ClientNumber;
    public readonly CompanyCode LegalUnit;

    public Policy()
    {
        LegalUnit = new CompanyCode();
    }

    public Policy(string? policyNumber, string? clientNumber, CompanyCode legalUnit)
    {
        PolicyNumber = policyNumber;
        ClientNumber = clientNumber;
        LegalUnit = legalUnit;

        var validator = new PolicyValidator(false);
        var result = validator.Validate(this);
        if (result.IsValid)
        {
            return;
        }

        throw new DomainValidationException(result.Errors);
    }
}