namespace API.Endpoints;

public static class BookingEndpoints
{
    public static async Task<IResult> GetAsync(int? id, IMediator mediator)
    {
        var res = await mediator.Send(new GetBookingByIdQuery { BookingId = id });

        return res.Match(
            result => Results.Ok(result.Value),
            validationError => new BaseErrorResponse(validationError.Value),
            _ => new BaseForbiddenResponse(),
            _ => new BaseNotFoundResponse()
        );
    }

    public static async Task<IResult> PostAsync([FromBody] InsertNewBookingCommand command, IMediator mediator)
    {
        var res = await mediator.Send(command);

        return res.Match(
            success => Results.Created("api/booking/" + success.Value.Id, success.Value),
            validationError => new BaseErrorResponse(validationError.Value),
            error => new BaseDomainErrorResponse(error.Value),
            _ => new BaseForbiddenResponse(),
            _ => new BaseDatabaseErrorResponse()
        );
    }

    public static async Task<IResult> PatchAsync(
        [FromRoute] int? id,
        [FromHeader(Name = "RowVersion")] string? rowVersion,
        [FromBody] UpdateBookingCommand command,
        IMediator mediator)
    {
        if (!Converters.TryConvertStringBase64ToBytes(rowVersion, out var bytes))
        {
            return new BaseFormatErrorResponse("RowVersion header cannot be converted to byte array.");
        }

        command.BookingId = id;
        command.RowVersion = bytes;
        var res = await mediator.Send(command);

        return res.Match(
            success => Results.Ok(success.Value),
            validationError => new BaseErrorResponse(validationError.Value),
            _ => new BaseConflictResponse(),
            error => new BaseDomainErrorResponse(error.Value),
            _ => new BaseForbiddenResponse(),
            _ => new BaseNotFoundResponse(),
            _ => new BaseDatabaseErrorResponse()
        );
    }

    public static async Task<IResult> DeleteAsync([FromRoute] int? id, IMediator mediator)
    {
        var res = await mediator.Send(new DeleteBookingByIdCommand { Id = id });

        return res.Match(
            _ => Results.NoContent(),
            validationError => new BaseErrorResponse(validationError.Value),
            error => new BaseDomainErrorResponse(error.Value),
            _ => new BaseForbiddenResponse(),
            _ => new BaseNotFoundResponse(),
            _ => new BaseDatabaseErrorResponse()
        );
    }

    public static async Task<IResult> SearchAsync(
        BookingSearchInput input,
        IMediator mediator
        )
    {
        var model = new SearchBookingsQuery
        {
            EndRow = input.EndRow,
            OrderBy = input.OrderBy,
            OrderByDirection = input.OrderByDirection,
            StartRow = input.StartRow,
            SearchItems = input.SearchItems
        };
        var res = await mediator.Send(model);

        return res.Match(
            result => Results.Ok(result.Value),
            _ => new BaseForbiddenResponse(),
            validationError => new BaseErrorResponse(validationError.Value)
        );
    }
}