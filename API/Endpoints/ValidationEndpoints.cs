namespace API.Endpoints;

public static class ValidationEndpoints
{
    public static async Task<IResult> ValidateNewAsync([FromBody] ValidateNewBookingCommand command, IMediator mediator)
    {
        var res = await mediator.Send(command);

        return res.Match(
            success => Results.Ok(success.Value),
            _ => Results.Forbid(),
            _ => new BaseErrorResponse(
                500,
                "External validation service error",
                "A error occurred when validating with external services. Check the logs for details."
            )
        );
    }

    public static async Task<IResult> ValidateByIdAsync([FromRoute] int? id, IMediator mediator)
    {
        var res = await mediator.Send(new ValidateBookingByIdCommand { Id = id });

        return res.Match(
            success => Results.Ok(success.Value),
            _ => Results.NotFound(),
            _ => Results.Forbid(),
            _ => new BaseErrorResponse(
                500,
                "External validation service error",
                "A error occurred when validating with external services. Check the logs for details."
            )
        );
    }

}