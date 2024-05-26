namespace API.Endpoints;

public static class ValidationEndpoints
{
    public static async Task<IResult> ValidateNewAsync([FromBody] ValidateNewBookingCommand command, IMediator mediator)
    {
        var res = await mediator.Send(command);

        return res.Match(
            success => Results.Ok(success.Value),
            _ => new BaseForbiddenResponse(),
            _ => new BaseDatabaseErrorResponse(),
            error => new BaseErrorResponse(400, "Format error", error.Value)
        );
    }

    public static async Task<IResult> ValidateByIdAsync([FromRoute] int? id, IMediator mediator)
    {
        var res = await mediator.Send(new ValidateBookingByIdCommand { Id = id });

        return res.Match(
            success => Results.Ok(success.Value),
            _ => new BaseNotFoundResponse(),
            _ => new BaseForbiddenResponse(),
            _ => new BaseDatabaseErrorResponse(),
            error => new BaseErrorResponse(400, "Format error", error.Value)
        );
    }

}