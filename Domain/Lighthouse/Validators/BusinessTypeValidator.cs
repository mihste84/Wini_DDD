namespace Domain.Lighthouse.Validators;
public class BusinessTypeValidator : AbstractValidator<BusinessType>
{
    public BusinessTypeValidator(bool isRequired = true)
    {
        RuleFor(_ => _.Code).MaximumLength(20).WithName("Business type");

        When(
            _ => isRequired,
            () =>
            {
                RuleFor(_ => _.Code)
                    .NotEmpty()
                    .WithName("Business type");
            });
    }
}