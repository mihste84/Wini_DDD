namespace Domain.Validators;
public class CompanyCodeValidator : AbstractValidator<CompanyCode>
{
    public CompanyCodeValidator()
    {
        RuleFor(_ => _.Code).NotEmpty().InclusiveBetween(0, 999).WithName("Company Code");
    }
}