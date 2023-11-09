namespace Domain.Validators;
public class CurrencyCodeValidator : AbstractValidator<CurrencyCode>
{
    public CurrencyCodeValidator()
    {
        RuleFor(_ => _.Code).MaximumLength(3).WithName("Currency Code");
    }
}