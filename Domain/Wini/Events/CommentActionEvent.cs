namespace Domain.Wini.Events;

public class CommentActionEvent(CrudAction action, Comment comment) : BaseDomainEvent("ChangeComment")
{
    public readonly Comment Comment = comment;
    public readonly CrudAction Action = action;
}