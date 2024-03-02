namespace API.Endpoints;

public static class BookingStatusEndpoints
{
    public static async Task<IResult> Patch([FromRoute] int? id, [FromBody] UpdateBookingStatusCommand command, IMediator mediator)
    {
        command.BookingId = id;
        var res = await mediator.Send(command);

        return res.Match(
            success => Results.Ok(success.Value),
            _ => new BaseErrorResponse(409, "Update conflict", "Item has already been updated by another user."),
            validationError => new BaseErrorResponse(validationError.Value),
            error => new BaseErrorResponse(400, "Domain error", error.Value),
            _ => Results.Forbid(),
            _ => Results.NotFound(),
            _ => new BaseErrorResponse(500, "Database error",
                "A database error occurred when trying to update booking status. Check the logs for details."
            )
        );
    }
}