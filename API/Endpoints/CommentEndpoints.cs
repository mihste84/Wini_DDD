
namespace API.Endpoints;

public static class CommentEndpoints
{
    public static async Task<IResult> PostAsync(
        [FromRoute] int? id,
        [FromBody] CommentInput model,
        IMediator mediator)
    {
        var command = new UpdateBookingCommentCommand
        {
            Action = Domain.Enums.CrudAction.Added,
            BookingId = id,
            Created = model.Created,
            Value = model.Value,
        };

        var res = await mediator.Send(command);

        return res.Match(
            success => Results.Created("api/booking/" + success.Value.Id, success.Value),
            validationError => new BaseErrorResponse(validationError.Value),
            error => new BaseDomainErrorResponse(error.Value),
            _ => new BaseNotFoundResponse(),
            _ => new BaseForbiddenResponse(),
            _ => new BaseDatabaseErrorResponse()
        );
    }

    public static async Task<IResult> PatchAsync(
        [FromRoute] int? id,
        [FromBody] CommentInput model,
        IMediator mediator)
    {
        var command = new UpdateBookingCommentCommand
        {
            Action = Domain.Enums.CrudAction.Edited,
            BookingId = id,
            Created = model.Created,
            Value = model.Value,
        };

        var res = await mediator.Send(command);

        return res.Match(
            success => Results.Ok(success.Value),
            validationError => new BaseErrorResponse(validationError.Value),
            error => new BaseDomainErrorResponse(error.Value),
            _ => new BaseNotFoundResponse(),
            _ => new BaseForbiddenResponse(),
            _ => new BaseDatabaseErrorResponse()
        );
    }

    public static async Task<IResult> DeleteAsync(
        [FromRoute] int? id,
        [FromQuery] DateTime? created,
        IMediator mediator
    )
    {
        var command = new UpdateBookingCommentCommand
        {
            Action = Domain.Enums.CrudAction.Deleted,
            BookingId = id,
            Created = created,
        };

        var res = await mediator.Send(command);

        return res.Match(
            success => Results.Ok(success.Value),
            validationError => new BaseErrorResponse(validationError.Value),
            error => new BaseDomainErrorResponse(error.Value),
            _ => new BaseNotFoundResponse(),
            _ => new BaseForbiddenResponse(),
            _ => new BaseDatabaseErrorResponse()
        );
    }
}