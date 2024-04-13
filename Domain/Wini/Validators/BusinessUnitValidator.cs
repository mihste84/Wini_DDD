namespace Domain.Wini.Validators;
public class BusinessUnitValidator : AbstractValidator<BusinessUnit>
{
    public BusinessUnitValidator(bool isRequired = true)
    {
        RuleFor(_ => _.ToString()).MaximumLength(12).WithName("Business unit");

        When(
            _ => isRequired,
            () =>
            {
                RuleFor(_ => _.ToString())
                    .NotEmpty()
                    .WithName("Business unit");
            });
    }
}