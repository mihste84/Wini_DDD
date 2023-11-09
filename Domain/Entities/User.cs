namespace Domain.Entities;

public class User
{
    public readonly string? UserId;

    public User(string? userId)
    {
        if (userId?.Length > 8)
        {
            throw new TextValidationException(
                nameof(userId),
                userId,
                ValidationErrorCodes.TextTooLong,
                "User ID cannot be longer than 8 characters"
            )
            { MaxLength = 8 };
        }

        UserId = userId;
    }
}