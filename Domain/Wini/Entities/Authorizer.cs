namespace Domain.Wini.Entities;

public class Authorizer : User
{
    public bool HasAuthorized { get; }
    public Authorizer() : base() { }
    public Authorizer(string? userId, bool hasAuthorized) : base(userId, false, "Authorizer")
    {
        HasAuthorized = hasAuthorized;

        var validator = new AuthorizerValidator(false);
        var result = validator.Validate(this);
        if (!result.IsValid)
            throw new DomainValidationException(result.Errors);
    }

    public Authorizer Authorize() => new(UserId, true);
}