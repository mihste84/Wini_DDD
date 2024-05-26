

namespace Domain.Lighthouse.Validators;
public class PolicyValidator : AbstractValidator<Policy>
{
    public PolicyValidator(bool isRequired = true)
    {
        RuleFor(_ => _.ClientNumber).MaximumLength(20).WithName("Client number");
        RuleFor(_ => _.PolicyNumber).MaximumLength(20).WithName("Policy number");
        RuleFor(_ => _.LegalUnit).SetValidator(new CompanyCodeValidator(isRequired));

        When(
            _ => isRequired,
            () =>
            {
                RuleFor(_ => _.ClientNumber)
                    .NotEmpty()
                    .WithName("Client number");
                RuleFor(_ => _.PolicyNumber)
                    .NotEmpty()
                    .WithName("Policy number");
            });
    }
}