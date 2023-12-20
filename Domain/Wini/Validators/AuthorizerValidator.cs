namespace Domain.Wini.Validators;
public class AuthorizerValidator : AbstractValidator<Authorizer>
{
    public AuthorizerValidator(bool isRequired = true)
    {
        When(_ => !string.IsNullOrWhiteSpace(_.UserId), () => RuleFor(_ => _.UserId).Must(_ => _?.Contains(';') == false));
        When(_ => _.HasAuthorized, () =>
            RuleFor(_ => _.UserId)
                .NotEmpty()
                .WithName("Authorizer")
                .WithMessage("Authorizer needs to be set when row is authorized.")
        );

        When(_ => isRequired, () =>
        {
            RuleFor(_ => _.UserId)
                .NotEmpty()
                .WithName("Authorizer");
        });
    }
}