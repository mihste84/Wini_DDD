namespace API.Endpoints;

public record AttachmentInput(byte[] RowVersion, IFormFileCollection? UploadedFiles);

public static class AttachmentEndpoints
{
    public static async Task<IResult> PostAsync(
        [FromRoute] int? id,
        [FromForm] AttachmentInput? input,
        IMediator mediator
    )
    {
        var files = input?.UploadedFiles?.Select(_ => new UploadedAttachmentInput(
            _.OpenReadStream(),
            _.FileName,
            _.Length,
            _.ContentType
        ));

        try
        {
            var command = new InsertAttachmentsCommand
            {
                BookingId = id,
                RowVersion = input?.RowVersion,
                Files = files
            };
            var res = await mediator.Send(command);

            return res.Match(
                success => Results.Created("api/booking/" + success.Value.Id, success.Value),
                validationError => new BaseErrorResponse(validationError.Value),
                _ => new BaseConflictResponse(),
                error => new BaseErrorResponse(400, "Domain error", error.Value),
                _ => new BaseNotFoundResponse(),
                _ => new BaseForbiddenResponse(),
                _ => new BaseDatabaseErrorResponse()
            );
        }
        finally
        {
            await DisposeAllStreamsAsync(files);
        }
    }

    public static async Task<IResult> DeleteAsync(
        [FromRoute] int? id,
        [FromQuery] string? fileName,
        [FromQuery] byte[]? rowVersion,
        IMediator mediator
    )
    {
        var command = new DeleteAttachmentCommand
        {
            BookingId = id,
            RowVersion = rowVersion,
            FileName = fileName
        };

        var res = await mediator.Send(command);

        return res.Match(
            success => Results.Ok(success.Value),
            validationError => new BaseErrorResponse(validationError.Value),
            _ => new BaseConflictResponse(),
            error => new BaseErrorResponse(400, "Domain error", error.Value),
            _ => new BaseNotFoundResponse(),
            _ => new BaseForbiddenResponse(),
            _ => new BaseDatabaseErrorResponse()
        );
    }

    private static async Task DisposeAllStreamsAsync(IEnumerable<UploadedAttachmentInput>? files)
    {
        if (files == default)
        {
            return;
        }

        foreach (var file in files)
        {
            await file.Content!.DisposeAsync();
        }
    }
}