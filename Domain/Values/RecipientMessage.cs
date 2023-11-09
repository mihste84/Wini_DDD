namespace Domain.Values;

public record RecipientMessage
{
    public readonly string Message;
    public readonly IdValue<int> BookingId;
    public readonly User Recipient;

    public RecipientMessage(string message, User recipient, IdValue<int> bookingId)
    {
        if (string.IsNullOrWhiteSpace(message))
        {
            throw new DomainLogicException(
                nameof(message),
                ValidationErrorCodes.Required,
                "Message cannot be empty when recipient is set"
            );
        }

        if (message.Length > 100)
        {
            throw new TextValidationException(
                nameof(message),
                message,
                ValidationErrorCodes.TextTooLong,
                "Message cannot be longer than 100 characters"
            )
            { MaxLength = 100 };
        }

        Message = message;
        Recipient = recipient;
        BookingId = bookingId;
    }
}