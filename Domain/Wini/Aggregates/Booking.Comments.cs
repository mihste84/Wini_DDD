namespace Domain.Wini.Aggregates;

public partial class Booking
{
    public Comment AddNewComment(string? comment, DateTime createdDate, IAuthenticationService authenticationService)
    {
        CheckIfCanComment(authenticationService);
        if (Comments.FindIndex(_ => _.Created.CompareWithoutMilliseconds(createdDate)) > -1)
            throw new DomainLogicException(nameof(createdDate), createdDate.ToString(), "Cannot have multiple comments with same created date.");

        var newComment = new Comment(comment, BookingId!, createdDate);
        Comments.Add(newComment);
        AddCommentEvent(CrudAction.Added, newComment);

        return newComment;
    }

    public void EditComment(string? comment, DateTime createdDate, IAuthenticationService authenticationService)
    {
        CheckIfCanComment(authenticationService);
        var index = GetCommentIndexCreateDate(createdDate);
        var newComment = new Comment(comment, BookingId!);
        Comments[index] = newComment;
        AddCommentEvent(CrudAction.Edited, newComment);
    }

    public void DeleteComment(DateTime createdDate, IAuthenticationService authenticationService)
    {
        CheckIfCanComment(authenticationService);
        var commentToRemove = GetCommentByCreateDate(createdDate);

        Comments.Remove(commentToRemove);
        AddCommentEvent(CrudAction.Deleted, commentToRemove);
    }

    private void CheckIfCanComment(IAuthenticationService authenticationService)
    {
        if (BookingId == default)
            throw new DomainLogicException("Cannot add comment on unsaved bookings.");

        var userId = authenticationService.GetUserId();
        if (userId != Commissioner.UserId)
            throw new DomainLogicException(nameof(userId), userId, "Only commissioners can add comments.");
    }

    private Comment GetCommentByCreateDate(DateTime createdDate)
    => Comments.Find(_ => _.Created.CompareWithoutMilliseconds(createdDate))
        ?? throw new DomainLogicException($"Cannot find comment created on date {createdDate}.");

    private int GetCommentIndexCreateDate(DateTime createdDate)
    {
        var index = Comments.FindIndex(_ => _.Created.CompareWithoutMilliseconds(createdDate));
        if (index == -1)
            throw new DomainLogicException($"Cannot find comment created on date {createdDate}.");

        return index;
    }

    private void AddCommentEvent(CrudAction action, Comment? newComment)
        => DomainEvents.Add(new CommentActionEvent(action, newComment));
}