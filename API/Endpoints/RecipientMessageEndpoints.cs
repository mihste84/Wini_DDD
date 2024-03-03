namespace API.Endpoints;

public static class RecipientMessageEndpoints
{
    public static async Task<IResult> PatchAsync([FromRoute] int? id, [FromBody] UpdateRecipientMessageCommand command, IMediator mediator)
    {
        command.BookingId = id;
        var res = await mediator.Send(command);

        return res.Match(
            success => Results.Ok(success.Value),
            validationError => new BaseErrorResponse(validationError.Value),
            _ => new BaseErrorResponse(409, "Update conflict", "Item has already been updated by another user."),
            error => new BaseErrorResponse(400, "Domain error", error.Value),
            _ => Results.NotFound(),
            _ => Results.Forbid(),
            _ => new BaseErrorResponse(
                500,
                "Database error",
                "A database error occurred when trying to update recipient. Check the logs for details."
            )
        );
    }
}