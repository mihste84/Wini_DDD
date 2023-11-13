namespace Domain.Validators;
public class CurrencyValidator : AbstractValidator<Currency>
{
    public CurrencyValidator(bool isRequired = true)
    {
        RuleFor(_ => _.Code).SetValidator(new CurrencyCodeValidator(isRequired));

        When(_ => isRequired, () =>
        {
            RuleFor(_ => _.Code)
                .NotEmpty()
                .WithName("Currency Rate");
        });
    }
}