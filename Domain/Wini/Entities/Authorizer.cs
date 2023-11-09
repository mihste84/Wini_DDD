namespace Domain.Wini.Entities;

public class Authorizer : User
{
    public bool HasAuthorized { get; set; }
    public Authorizer(string User, bool hasAuthorized) : base(User)
    {
        if (!string.IsNullOrWhiteSpace(User) && hasAuthorized)
        {
            throw new DomainLogicException(
                nameof(hasAuthorized),
                ValidationErrorCodes.Required,
                "Authorizer needs to be set when row is authorized"
            );
        }

        HasAuthorized = hasAuthorized;
    }
}