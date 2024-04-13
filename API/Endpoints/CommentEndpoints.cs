
namespace API.Endpoints;

public static class CommentEndpoints
{
    public static async Task<IResult> PostAsync(
        [FromRoute] int? id,
        [FromHeader(Name = "RowVersion")] string? rowVersionString,
        [FromBody] CommentInput model,
        IMediator mediator)
    {
        if (!Converters.TryConvertStringBase64ToBytes(rowVersionString, out var rowVersion))
        {
            return new BaseFormatErrorRespons("RowVersion header cannot be converted to byte array.");
        }

        var command = new UpdateBookingCommentCommand
        {
            Action = Domain.Wini.Events.CrudAction.Added,
            BookingId = id,
            Created = model.Created,
            Value = model.Value,
            RowVersion = rowVersion
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

    public static async Task<IResult> PatchAsync(
        [FromRoute] int? id,
        [FromHeader(Name = "RowVersion")] string? rowVersionString,
        [FromBody] CommentInput model,
        IMediator mediator)
    {
        if (!Converters.TryConvertStringBase64ToBytes(rowVersionString, out var rowVersion))
        {
            return new BaseFormatErrorRespons("RowVersion header cannot be converted to byte array.");
        }

        var command = new UpdateBookingCommentCommand
        {
            Action = Domain.Wini.Events.CrudAction.Edited,
            BookingId = id,
            Created = model.Created,
            Value = model.Value,
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

    public static async Task<IResult> DeleteAsync(
        [FromRoute] int? id,
        [FromHeader(Name = "RowVersion")] string? rowVersionString,
        [FromQuery] DateTime? created,
        IMediator mediator
    )
    {
        if (!Converters.TryConvertStringBase64ToBytes(rowVersionString, out var rowVersion))
        {
            return new BaseFormatErrorRespons("RowVersion header cannot be converted to byte array.");
        }

        var command = new UpdateBookingCommentCommand
        {
            Action = Domain.Wini.Events.CrudAction.Deleted,
            BookingId = id,
            Created = created,
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