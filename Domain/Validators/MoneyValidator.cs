namespace Domain.Validators;
public class MoneyValidator : AbstractValidator<Money>
{
    public MoneyValidator()
    {
        RuleFor(_ => _.Currency).SetValidator(new CurrencyValidator()!);
        RuleFor(_ => _.Amount).NotEmpty();
    }
}