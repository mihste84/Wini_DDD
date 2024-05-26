namespace Domain.Wini.Events;

public class RecipinetMessageActionEvent(CrudAction action, RecipientMessage msg) : BaseDomainEvent("ChangeComment")
{
    public readonly RecipientMessage RecipientMessage = msg;
    public readonly CrudAction Action = action;
}