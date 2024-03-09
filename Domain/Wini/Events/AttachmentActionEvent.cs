namespace Domain.Wini.Events;

public class AttachmentActionEvent(CrudAction action, Attachment attachment) : BaseDomainEvent("ChangeAttachment")
{
    public readonly Attachment Attachment = attachment;
    public readonly CrudAction Action = action;
}