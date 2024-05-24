
namespace Domain.Lighthouse.Values;

public readonly record struct Policy
{
    public readonly string? PolicyNumber;
    public readonly string? ClientNumber;
    public readonly CompanyCode LegalUnit;

}