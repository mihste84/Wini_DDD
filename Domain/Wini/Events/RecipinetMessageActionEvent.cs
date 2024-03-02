namespace Domain.Wini.Events;

public class RecipinetMessageActionEvent : BaseDomainEvent
{
    public readonly RecipientMessage? RecipientMessage;
    public readonly CrudAction Action;
    public RecipinetMessageActionEvent(CrudAction action, RecipientMessage? msg) : base("ChangeComment")
    {
        RecipientMessage = msg;
        Action = action;
    }
}