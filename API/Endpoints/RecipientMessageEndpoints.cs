namespace API.Endpoints;

public static class RecipientMessageEndpoints
{
    public static async Task<IResult> PostAsync([FromRoute] int? id, [FromBody] RecipientMessageInput input, IMediator mediator)
    {
        var command = new UpdateRecipientMessageCommand {
            Action = Domain.Wini.Events.CrudAction.Added,
            Value = input.Value,
            RowVersion = input.RowVersion,
            BookingId = id,
            Recipient = input.Recipient
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
                "A database error occurred when trying to insert recipient. Check the logs for details."
            )
        );
    }

    public static async Task<IResult> PatchAsync([FromRoute] int? id, [FromBody] RecipientMessageInput input, IMediator mediator)
    {
        var command = new UpdateRecipientMessageCommand {
            Action = Domain.Wini.Events.CrudAction.Edited,
            Value = input.Value,
            RowVersion = input.RowVersion,
            BookingId = id,
            Recipient = input.Recipient
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
                "A database error occurred when trying to update recipient. Check the logs for details."
            )
        );
    }

    public static async Task<IResult> DeleteAsync(
        [FromRoute] int? id,
        [FromQuery] byte[] rowVersion,
        [FromQuery] string? recipient,
        IMediator mediator
    )
    {
        var command = new UpdateRecipientMessageCommand {
            Action = Domain.Wini.Events.CrudAction.Deleted,
            BookingId = id,
            Recipient = recipient,
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
                "A database error occurred when trying to delete recipient. Check the logs for details."
            )
        );
    }
}