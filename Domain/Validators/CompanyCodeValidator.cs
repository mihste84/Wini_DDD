namespace Domain.Validators;
public class CompanyCodeValidator : AbstractValidator<CompanyCode>
{
    public CompanyCodeValidator(bool isRequired = true)
    {
        RuleFor(_ => _.Code).InclusiveBetween(0, 999).WithName("Company Code");

        When(
            _ => isRequired,
            () =>
            {
                RuleFor(_ => _.Code)
                    .NotEmpty()
                    .WithName("Company Code");
            });
    }
}