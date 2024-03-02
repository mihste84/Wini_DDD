namespace Domain.Wini.Values;

public record RecipientMessage
{
    public readonly string Message;
    public readonly IdValue<int> BookingId;
    public readonly User Recipient;

    public RecipientMessage(string message, User recipient, IdValue<int> bookingId)
    {
        Message = message;
        Recipient = recipient;
        BookingId = bookingId;

        var validator = new RecipientMessageValidator();
        var result = validator.Validate(this);
        if (!result.IsValid)
            throw new DomainValidationException(result.Errors);
    }

    public RecipientMessage(string message, string recipient, IdValue<int> bookingId)
    {
        Message = message;
        Recipient = new User(recipient);
        BookingId = bookingId;

        var validator = new RecipientMessageValidator();
        var result = validator.Validate(this);
        if (!result.IsValid)
            throw new DomainValidationException(result.Errors);
    }
}