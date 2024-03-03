namespace Domain.Wini.Aggregates;

public partial class Booking
{
    public void AddNewRecipientMessage(string msg, string recipient, IAuthenticationService authenticationService)
    {
        CheckIfCanChangeRecipientMessage(authenticationService);
        if (Messages.FindIndex(_ => _.Recipient.UserId == recipient) > -1)
        {
            throw new DomainLogicException(nameof(recipient), recipient, "Cannot have multiple messages with same recipient.");
        }

        var newRecipient = new RecipientMessage(msg, recipient, BookingId!);
        Messages.Add(newRecipient);
        AddRecipientMessageEvent(CrudAction.Added, newRecipient);
    }

    public void EditRecipientMessage(string msg, string recipient, IAuthenticationService authenticationService)
    {
        CheckIfCanChangeRecipientMessage(authenticationService);
        var index = GetecipientMessageIndexByUser(recipient);
        var newRecipient = new RecipientMessage(msg, recipient, BookingId!);
        Messages[index] = newRecipient;
        AddRecipientMessageEvent(CrudAction.Edited, newRecipient);
    }

    public void DeleteRecipientMessage(string recipient, IAuthenticationService authenticationService)
    {
        CheckIfCanChangeRecipientMessage(authenticationService);
        var recipientToRemove = GetRecipientMessageByUser(recipient);
        Messages.Remove(recipientToRemove);
        AddRecipientMessageEvent(CrudAction.Deleted, recipientToRemove);
    }

    private void CheckIfCanChangeRecipientMessage(IAuthenticationService authenticationService)
    {
        if (BookingId == default)
        {
            throw new DomainLogicException("Cannot change recipients on unsaved bookings.");
        }

        if (BookingStatus.Status != WiniStatus.Saved)
        {
            throw new DomainLogicException("Recipients can only be changed when status is 'Saved'.");
        }

        var userId = authenticationService.GetUserId();
        if (userId == Commissioner.UserId)
        {
            return;
        }

        throw new DomainLogicException(nameof(userId), userId, "Only commissioners can change recipients.");
    }

    private RecipientMessage GetRecipientMessageByUser(string? recipient)
    => Messages.Find(_ => _.Recipient.UserId == recipient)
        ?? throw new DomainLogicException($"Cannot find message with recipient {recipient}.");

    private int GetecipientMessageIndexByUser(string recipient)
    {
        var index = Messages.FindIndex(_ => _.Recipient.UserId == recipient);
        if (index == -1)
        {
            throw new DomainLogicException($"Cannot find message with recipient {recipient}.");
        }

        return index;
    }

    private void AddRecipientMessageEvent(CrudAction action, RecipientMessage? recipient)
        => DomainEvents.Add(new RecipinetMessageActionEvent(action, recipient));
}