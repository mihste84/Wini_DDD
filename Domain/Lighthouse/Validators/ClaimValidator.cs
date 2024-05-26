

namespace Domain.Lighthouse.Validators;
public class ClaimValidator : AbstractValidator<Claim>
{
    public ClaimValidator(bool isRequired = true)
    {
        RuleFor(_ => _.ClaimEvent).MaximumLength(20).WithName("Claim event");
        RuleFor(_ => _.ClaimNumber).MaximumLength(20).WithName("Claim number");
        RuleFor(_ => _.ClaimYear).ExclusiveBetween(1900, 2099).WithName("Claim year");

        When(
            _ => isRequired,
            () =>
            {
                RuleFor(_ => _.ClaimEvent)
                    .NotEmpty()
                    .WithName("Claim event");
                RuleFor(_ => _.ClaimNumber)
                    .NotEmpty()
                    .WithName("Claim number");
                RuleFor(_ => _.ClaimYear)
                    .NotEmpty()
                    .WithName("Claim year");
            });
    }
}