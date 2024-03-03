namespace Domain.Wini.Validators;
public class AccountValidator : AbstractValidator<Account>
{
    public AccountValidator(bool isRequired = true)
    {
        When(_ => !string.IsNullOrWhiteSpace(_.Value), () => RuleFor(_ => _.Value).Must(_ => _?.Contains(';') == false));
        RuleFor(_ => _.Value)
            .MaximumLength(6)
            .WithName("Account");

        When(_ => !string.IsNullOrWhiteSpace(_.Subsidiary), () => RuleFor(_ => _.Value).Must(_ => _?.Contains(';') == false));
        RuleFor(_ => _.Subsidiary)
            .MaximumLength(8);

        When(
            _ => isRequired,
            () =>
            {
                RuleFor(_ => _.Value)
                    .NotEmpty()
                    .WithName("Account");
            });
    }
}