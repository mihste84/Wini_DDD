namespace API.Endpoints;

public static class HelloEndpoints
{
    public static async Task<IResult> Post([FromBody] InsertHelloCommand command, IMediator mediator)
    {
        var res = await mediator.Send(command);

        return res.Match(
            success => Results.Created("api/hello/" + success.Value, success.Value),
            validationError => new BaseErrorResponse(validationError.Value)
        );
    }

    public static async Task<IResult> Patch(
        [FromRoute] int? id,
        [FromBody] UpdateHelloInput? body,
        IMediator mediator
    )
    {
        var res = await mediator.Send(new UpdateHelloCommand { Id = id, Input = body?.Input, RowVersion = body?.RowVersion });

        return res.Match(
            success => Results.Ok(success.Value),
            validationError => new BaseErrorResponse(validationError.Value),
            error => new BaseErrorResponse(409, "Update conflict", error.Value)
        );
    }

    public static async Task<IResult> Delete([FromRoute] int? id, IMediator mediator)
    {
        var res = await mediator.Send(new DeleteHelloCommand { Id = id });

        return res.Match(
            success => Results.NoContent(),
            no => new BaseErrorResponse(400, "Unable to delete", "The item could not be deleted. Its either already deleted or the input ID is incorrect."),
            validationError => new BaseErrorResponse(validationError.Value)
        );
    }
}