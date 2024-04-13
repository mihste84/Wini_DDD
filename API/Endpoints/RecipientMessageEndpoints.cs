namespace API.Endpoints;

public static class RecipientMessageEndpoints
{
    public static async Task<IResult> PostAsync([FromRoute] int? id, [FromBody] RecipientMessageInput input, IMediator mediator)
    {
        var command = new UpdateRecipientMessageCommand
        {
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
            _ => new BaseConflictResponse(),
            error => new BaseErrorResponse(400, "Domain error", error.Value),
            _ => new BaseNotFoundResponse(),
            _ => new BaseForbiddenResponse(),
            _ => new BaseDatabaseErrorResponse()
        );
    }

    public static async Task<IResult> PatchAsync([FromRoute] int? id, [FromBody] RecipientMessageInput input, IMediator mediator)
    {
        var command = new UpdateRecipientMessageCommand
        {
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
            _ => new BaseConflictResponse(),
            error => new BaseErrorResponse(400, "Domain error", error.Value),
            _ => new BaseNotFoundResponse(),
            _ => new BaseForbiddenResponse(),
            _ => new BaseDatabaseErrorResponse()

        );
    }

    public static async Task<IResult> DeleteAsync(
        [FromRoute] int? id,
        [FromQuery] byte[] rowVersion,
        [FromQuery] string? recipient,
        IMediator mediator
    )
    {
        var command = new UpdateRecipientMessageCommand
        {
            Action = Domain.Wini.Events.CrudAction.Deleted,
            BookingId = id,
            Recipient = recipient,
            RowVersion = rowVersion
        };

        var res = await mediator.Send(command);

        return res.Match(
            success => Results.Ok(success.Value),
            validationError => new BaseErrorResponse(validationError.Value),
            _ => new BaseConflictResponse(),
            error => new BaseErrorResponse(400, "Domain error", error.Value),
            _ => new BaseNotFoundResponse(),
            _ => new BaseForbiddenResponse(),
            _ => new BaseDatabaseErrorResponse()
        );
    }
}