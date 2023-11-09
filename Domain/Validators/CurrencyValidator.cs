namespace Domain.Validators;
public class CurrencyValidator : AbstractValidator<Currency>
{
    public CurrencyValidator()
    {
        RuleFor(_ => _.Code).SetValidator(new CurrencyCodeValidator());
        RuleFor(_ => _.Code).NotNull();
        RuleFor(_ => _.CurrencyRate).NotEmpty();
    }
}