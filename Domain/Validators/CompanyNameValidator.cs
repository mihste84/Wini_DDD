namespace Domain.Validators;
public class CompanyNameValidator : AbstractValidator<CompanyName>
{
    public CompanyNameValidator()
    {
        RuleFor(_ => _.Name).MaximumLength(50).NotEmpty().WithName("Company Name");
    }
}