using Domain.Wini.Enums;

namespace API.Endpoints;

public static class BookingStatusEndpoints
{
    public static async Task<IResult> PatchAsync(
        [FromRoute] int? id,
        [FromHeader(Name = "RowVersion")] string? rowVersion,
        [FromRoute] WiniStatus status,
        IMediator mediator
    )
    {
        if (!Converters.TryConvertStringBase64ToBytes(rowVersion, out var bytes))
        {
            return new BaseFormatErrorResponse("RowVersion header cannot be converted to byte array.");
        }

        var command = new UpdateBookingStatusCommand
        {
            BookingId = id,
            RowVersion = bytes,
            Status = status
        };
        var res = await mediator.Send(command);

        return res.Match(
            success => Results.Ok(success.Value),
            _ => new BaseConflictResponse(),
            validationError => new BaseErrorResponse(validationError.Value),
            error => new BaseDomainErrorResponse(error.Value),
            _ => new BaseForbiddenResponse(),
            _ => new BaseNotFoundResponse(),
            _ => new BaseDatabaseErrorResponse()
        );
    }
}