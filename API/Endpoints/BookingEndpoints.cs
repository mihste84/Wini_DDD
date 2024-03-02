namespace API.Endpoints;

public static class BookingEndpoints
{
    public static async Task<IResult> Get(int? id, IMediator mediator)
    {
        var res = await mediator.Send(new GetBookingByIdQuery { BookingId = id });

        return res.Match(
            result => Results.Ok(result.Value),
            validationError => new BaseErrorResponse(validationError.Value),
            _ => Results.Forbid(),
            _ => Results.NotFound()
        );
    }

    public static async Task<IResult> Post([FromBody] InsertNewBookingCommand command, IMediator mediator)
    {
        var res = await mediator.Send(command);

        return res.Match(
            success => Results.Created("api/booking/" + success.Value.Id, success.Value),
            validationError => new BaseErrorResponse(validationError.Value),
            error => new BaseErrorResponse(400, "Domain error", error.Value),
            _ => Results.Forbid(),
            _ => new BaseErrorResponse(500, "Database error",
                "A database error occurred when trying to insert new booking. Check the logs for details."
            )
        );
    }

    public static async Task<IResult> Patch([FromRoute] int? id, [FromBody] UpdateBookingCommand command, IMediator mediator)
    {
        command.BookingId = id;
        var res = await mediator.Send(command);

        return res.Match(
            success => Results.Ok(success.Value),
            validationError => new BaseErrorResponse(validationError.Value),
            _ => new BaseErrorResponse(409, "Update conflict", "Item has already been updated by another user."),
            error => new BaseErrorResponse(400, "Domain error", error.Value),
            _ => Results.Forbid(),
            _ => Results.NotFound(),
            _ => new BaseErrorResponse(500, "Database error",
                "A database error occurred when trying to update booking. Check the logs for details."
            )
        );
    }

    public static async Task<IResult> Delete([FromRoute] int? id, IMediator mediator)
    {
        var res = await mediator.Send(new DeleteBookingByIdCommand { Id = id });

        return res.Match(
            _ => Results.NoContent(),
            validationError => new BaseErrorResponse(validationError.Value),
            error => new BaseErrorResponse(400, "Domain error", error.Value),
            _ => Results.Forbid(),
            _ => Results.NotFound(),
            _ => new BaseErrorResponse(500, "Database error",
                "A database error occurred when trying to delete booking. Check the logs for details."
            )
        );
    }
}