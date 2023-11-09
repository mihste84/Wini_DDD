namespace Domain.Wini.Entities;

public class Commissioner : User
{
    public Commissioner(string User) : base(User)
    {
        if (string.IsNullOrWhiteSpace(User))
        {
            throw new TextValidationException(
                nameof(User), User, ValidationErrorCodes.Required, "Commissioner cannot be empty"
            );
        }
    }
}
