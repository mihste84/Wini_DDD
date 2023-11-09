namespace Domain.Values;

public record Description
{
    public string Message { get; }

    public Description(string message)
    {
        if (!string.IsNullOrWhiteSpace(message) && message.Length > 30)
        {
            throw new TextValidationException(
                nameof(message),
                message,
                ValidationErrorCodes.TextTooLong,
                "Description cannot be longer than 30 characters"
            )
            { MaxLength = 30 };
        }

        Message = message;
    }
}