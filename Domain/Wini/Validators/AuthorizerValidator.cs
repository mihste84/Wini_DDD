namespace Domain.Wini.Validators;
public class AuthorizerValidator : AbstractValidator<Authorizer>
{
    public AuthorizerValidator()
    {
        When(_ => _.HasAuthorized, () =>
            RuleFor(_ => _.UserId).NotEmpty().WithName("Authorizer").WithMessage("Authorizer needs to be set when row is authorized.")
        );
    }
}