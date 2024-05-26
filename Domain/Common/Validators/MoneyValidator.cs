namespace Domain.Common.Validators;
public class MoneyValidator : AbstractValidator<Money>
{
    public MoneyValidator(bool isRequired = true)
    {
        SetBaseRules(isRequired);
    }

    public void SetBaseRules(bool isRequired)
    {
        RuleFor(_ => _.Currency).SetValidator(new CurrencyValidator(isRequired));

        When(
            _ => isRequired,
            () =>
            {
                RuleFor(_ => _.Amount)
                    .NotEmpty()
                    .NotEqual(_ => 0)
                    .WithName("Amount");
            });
    }
}