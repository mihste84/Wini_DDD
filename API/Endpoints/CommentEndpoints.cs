namespace API.Endpoints;

public static class CommentEndpoints
{
    public static async Task<IResult> PostAsync([FromRoute] int? id, [FromBody] CommentInput model, IMediator mediator)
    {
        var command = new UpdateBookingCommentCommand {
            Action = Domain.Wini.Events.CrudAction.Added,
            BookingId = id,
            Created = model.Created,
            Value = model.Value,
            RowVersion = model.RowVersion
        };

        var res = await mediator.Send(command);

        return res.Match(
            success => Results.Created("api/booking/" + success.Value.Id, success.Value),
            validationError => new BaseErrorResponse(validationError.Value),
            _ => new BaseErrorResponse(409, "Update conflict", "Item has already been updated by another user."),
            error => new BaseErrorResponse(400, "Domain error", error.Value),
            _ => Results.NotFound(),
            _ => Results.Forbid(),
            _ => new BaseErrorResponse(
                500,
                "Database error",
                "A database error occurred when trying to add comment. Check the logs for details."
            )
        );
    }

    public static async Task<IResult> PatchAsync([FromRoute] int? id, [FromBody] CommentInput model, IMediator mediator)
    {
        var command = new UpdateBookingCommentCommand {
            Action = Domain.Wini.Events.CrudAction.Edited,
            BookingId = id,
            Created = model.Created,
            Value = model.Value,
            RowVersion = model.RowVersion
        };

        var res = await mediator.Send(command);

        return res.Match(
            success => Results.Ok(success.Value),
            validationError => new BaseErrorResponse(validationError.Value),
            _ => new BaseErrorResponse(409, "Update conflict", "Item has already been updated by another user."),
            error => new BaseErrorResponse(400, "Domain error", error.Value),
            _ => Results.NotFound(),
            _ => Results.Forbid(),
            _ => new BaseErrorResponse(
                500,
                "Database error",
                "A database error occurred when trying to update comment. Check the logs for details."
            )
        );
    }

    public static async Task<IResult> DeleteAsync(
        [FromRoute] int? id,
        [FromQuery] byte[] rowVersion,
        [FromQuery] DateTime? created,
        IMediator mediator
    )
    {
        var command = new UpdateBookingCommentCommand {
            Action = Domain.Wini.Events.CrudAction.Deleted,
            BookingId = id,
            Created = created,
            RowVersion = rowVersion
        };

        var res = await mediator.Send(command);

        return res.Match(
            success => Results.Ok(success.Value),
            validationError => new BaseErrorResponse(validationError.Value),
            _ => new BaseErrorResponse(409, "Update conflict", "Item has already been updated by another user."),
            error => new BaseErrorResponse(400, "Domain error", error.Value),
            _ => Results.NotFound(),
            _ => Results.Forbid(),
            _ => new BaseErrorResponse(
                500,
                "Database error",
                "A database error occurred when trying to add comment. Check the logs for details."
            )
        );
    }
}