namespace API.Endpoints;

public static class BookingStatusEndpoints
{
    public static async Task<IResult> PatchAsync([FromRoute] int? id, [FromBody] UpdateBookingStatusCommand command, IMediator mediator)
    {
        command.BookingId = id;
        var res = await mediator.Send(command);

        return res.Match(
            success => Results.Ok(success.Value),
            _ => new BaseConflictResponse(),
            validationError => new BaseErrorResponse(validationError.Value),
            error => new BaseErrorResponse(400, "Domain error", error.Value),
            _ => new BaseForbiddenResponse(),
            _ => new BaseNotFoundResponse(),
            _ => new BaseDatabaseErrorResponse()
        );
    }
}