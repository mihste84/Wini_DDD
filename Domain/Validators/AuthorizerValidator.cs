namespace Domain.Validators;
public class AuthorizerValidator : AbstractValidator<Authorizer>
{
    public AuthorizerValidator()
    {
        RuleFor(_ => _.UserId).MaximumLength(8);
    }
}