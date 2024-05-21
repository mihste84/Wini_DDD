namespace API.Endpoints;

public static class RecipientMessageEndpoints
{
    public static async Task<IResult> PostAsync(
        [FromRoute] int? id,
        [FromBody] RecipientMessageInput input,
        IMediator mediator)
    {
        var command = new UpdateRecipientMessageCommand
        {
            Action = Domain.Enums.CrudAction.Added,
            Value = input.Message,
            BookingId = id,
            Recipient = input.Recipient
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
        [FromBody] RecipientMessageInput input,
        IMediator mediator)
    {
        var command = new UpdateRecipientMessageCommand
        {
            Action = Domain.Enums.CrudAction.Edited,
            Value = input.Message,
            BookingId = id,
            Recipient = input.Recipient
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
        [FromQuery] string? recipient,
        IMediator mediator
    )
    {
        var command = new UpdateRecipientMessageCommand
        {
            Action = Domain.Enums.CrudAction.Deleted,
            BookingId = id,
            Recipient = recipient
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