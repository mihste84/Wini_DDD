namespace Domain.Entities;

public class Authorizer : User
{
    public bool HasAuthorized { get; set; }
    public Authorizer(string? userId, bool hasAuthorized) : base(userId, false, "Authorizer")
    {
        HasAuthorized = hasAuthorized;

        var validator = new AuthorizerValidator();
        var result = validator.Validate(this);
        if (!result.IsValid)
            throw new DomainValidationException(result.Errors);
    }
}