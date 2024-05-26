
namespace Domain.Lighthouse.Values;

public readonly record struct Claim
{
    public readonly string? ClaimNumber;
    public readonly string? ClaimEvent;
    public readonly int? ClaimYear;

    public Claim()
    {
    }

    public Claim(string? claimNumber, string? claimEvent, int? claimYear)
    {
        ClaimNumber = claimNumber;
        ClaimEvent = claimEvent;
        ClaimYear = claimYear;

        var validator = new ClaimValidator(false);
        var result = validator.Validate(this);
        if (result.IsValid)
        {
            return;
        }

        throw new DomainValidationException(result.Errors);
    }
}