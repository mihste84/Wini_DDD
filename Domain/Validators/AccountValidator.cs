namespace Domain.Validators;
public class AccountValidator : AbstractValidator<Account>
{
    public AccountValidator(bool isRequired = true)
    {
        RuleFor(_ => _.Value)
            .MaximumLength(6)
            .WithName("Account");
        RuleFor(_ => _.Subsidiary).MaximumLength(8);

        When(_ => isRequired, () =>
        {
            RuleFor(_ => _.Value)
                .NotEmpty()
                .WithName("Account");
        });
    }
}