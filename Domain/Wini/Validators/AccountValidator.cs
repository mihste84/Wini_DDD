namespace Domain.Wini.Validators;
public class AccountValidator : AbstractValidator<Account>
{
    public AccountValidator()
    {
        RuleFor(_ => _.Value).NotEmpty().MaximumLength(6).WithName("Account");
        RuleFor(_ => _.Subsidiary).MaximumLength(8);
    }
}