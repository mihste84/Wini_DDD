namespace Domain.Wini.Validators;
public class CurrencyValidator : AbstractValidator<Currency>
{
    public CurrencyValidator(bool isRequired = true)
    {
        RuleFor(_ => _.CurrencyCode).SetValidator(new CurrencyCodeValidator(isRequired));

        When(_ => isRequired, () =>
        {
            RuleFor(_ => _.CurrencyCode)
                .NotEmpty()
                .WithName("Currency Rate");
        });
    }
}