namespace Domain.Wini.Aggregates;

public partial class Booking {
    public void AddNewAttachment(
        string fileName,
        string contentType,
        string path,
        long size,
        IAuthenticationService authenticationService
    ) {
        CanChangeAttachments(authenticationService);

        if (Attachments.Count >= 5)
        {
            throw new DomainLogicException("Booking cannot contain more than 5 attachments.");
        }

        if (Attachments.Exists(_ => _.Content.Name == fileName))
        {
            throw new DomainLogicException(nameof(fileName), fileName, "Cannot add attachments with duplicate name.");
        }

        var newAttachment = new Attachment(
            BookingType.Wini,
            new IdValue<int>(BookingId!.Value),
            new FileContent(size, contentType, fileName, path)
        );

        Attachments.Add(newAttachment);
        AddAttachmentEvent(CrudAction.Added, newAttachment);
    }

    public void DeleteAttachment(
        string name,
        IAuthenticationService authenticationService
    ) {
        CanChangeAttachments(authenticationService);

        if (Attachments.Count == 0)
        {
            throw new DomainLogicException("Booking does not contain any attachments.");
        }

        var attachmentToDelete = Attachments.Find(_ => _.Content.Name == name)
            ?? throw new NotFoundException("Cannot find attachment to delete.");

        Attachments.Remove(attachmentToDelete);
        AddAttachmentEvent(CrudAction.Deleted, attachmentToDelete);
    }

    private void CanChangeAttachments(IAuthenticationService authenticationService)
    {
        if (BookingStatus.Status != WiniStatus.Saved)
        {
            throw new DomainLogicException("Cannot save attachemnts when status is not Saved.");
        }

        if (BookingId == default)
        {
            throw new DomainLogicException("Booking must be saved before adding attachment.");
        }

        if (authenticationService.GetUserId() != Commissioner.UserId)
        {
            throw new DomainLogicException("Only commissioner can save attachments.");
        }
    }

    private void AddAttachmentEvent(CrudAction action, Attachment attachment)
    => DomainEvents.Add(new AttachmentActionEvent(action, attachment));
}