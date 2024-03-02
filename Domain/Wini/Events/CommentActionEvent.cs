namespace Domain.Wini.Events;

public class CommentActionEvent : BaseDomainEvent
{
    public readonly Comment? Comment;
    public readonly CrudAction Action;
    public CommentActionEvent(CrudAction action, Comment? comment) : base("ChangeComment")
    {
        Comment = comment;
        Action = action;
    }
}