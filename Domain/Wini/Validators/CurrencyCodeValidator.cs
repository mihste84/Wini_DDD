namespace Domain.Wini.Validators;
public class CurrencyCodeValidator : AbstractValidator<CurrencyCode>
{
    public CurrencyCodeValidator(bool isRequired = true)
    {
        RuleFor(_ => _.Code).MaximumLength(3).WithName("Currency Code");

        When(
            _ => isRequired,
            () =>
            {
                RuleFor(_ => _.Code)
                    .NotEmpty()
                    .WithName("Currency Code");
            });
    }
}