namespace Domain.Wini.Validators;
public class MoneyValidator : AbstractValidator<Money>
{
    public MoneyValidator(bool isRequired = true)
    {
        RuleFor(_ => _.Currency).SetValidator(new CurrencyValidator(isRequired));

        When(_ => isRequired, () =>
        {
            RuleFor(_ => _.Amount)
                .NotEmpty();
        });
    }
}