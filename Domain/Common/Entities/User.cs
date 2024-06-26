namespace Domain.Common.Entities;

public class User
{
    public readonly string? UserId;

    public User()
    {
    }

    public User(string? userId)
    {
        UserId = userId;

        var validator = new UserValidator(false);
        var result = validator.Validate(this);
        if (result.IsValid)
        {
            return;
        }

        throw new DomainValidationException(result.Errors);
    }

    public User(string? userId, bool isRequired, string propertyName)
    {
        UserId = userId;

        var validator = new UserValidator(isRequired, propertyName);
        var result = validator.Validate(this);
        if (result.IsValid)
        {
            return;
        }

        throw new DomainValidationException(result.Errors);
    }
}