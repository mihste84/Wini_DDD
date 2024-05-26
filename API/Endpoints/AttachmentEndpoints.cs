using AppLogig.Requests;

namespace API.Endpoints;

public static class AttachmentEndpoints
{
    public static async Task<IResult> PostAsync(
        [FromRoute] int? id,
        [FromForm] IFormFileCollection? uploadedFiles,
        IAttachmentService attachmentService,
        IMediator mediator
    )
    {
        if (uploadedFiles == default)
        {
            return new BaseErrorResponse(400, "No files uploaded", "At least one file must be uploaded");
        }
        if (uploadedFiles.Count > 5)
        {
            return new BaseErrorResponse(400, "Too many files", "Cannot upload more than 5 files.");
        }

        var files = new List<UploadedAttachmentInput>();
        foreach (var file in uploadedFiles)
        {
            using var stream = file.OpenReadStream();
            var uploadResult = await attachmentService.SaveAttachmentAsync(stream, file.Name);
            if (!uploadResult.Success)
                return new BaseErrorResponse(500, "Error saving file", $"An error occurred while saving file {file.Name}.");

            files.Add(new UploadedAttachmentInput(
                file.FileName,
                file.Length,
                file.ContentType,
                uploadResult.Path
            ));
        }

        var command = new InsertAttachmentsCommand
        {
            BookingId = id,
            Files = files
        };
        var res = await mediator.Send(command);

        return res.Match(
            success => Results.Created("api/booking/" + success.Value.Id, success.Value),
            validationError => new BaseErrorResponse(validationError.Value),
            error => new BaseDomainErrorResponse(error.Value),
            _ => new BaseNotFoundResponse(),
            _ => new BaseForbiddenResponse(),
            _ => new BaseDatabaseErrorResponse()
        );
    }

    public static async Task<IResult> DeleteAsync(
        [FromRoute] int? id,
        [FromQuery] string? fileName,
        IMediator mediator
    )
    {
        var command = new DeleteAttachmentCommand
        {
            BookingId = id,
            FileName = fileName
        };

        var res = await mediator.Send(command);

        return res.Match(
            success => Results.Ok(success.Value),
            validationError => new BaseErrorResponse(validationError.Value),
            error => new BaseDomainErrorResponse(error.Value),
            _ => new BaseNotFoundResponse(),
            _ => new BaseForbiddenResponse(),
            _ => new BaseDatabaseErrorResponse()
        );
    }

    public static async Task<IResult> GetAsync(
        [FromRoute] int? id,
        [FromQuery] string? fileName,
        IAttachmentService attachmentService,
        IMediator mediator
    )
    {
        var command = new GetAttachmentByNameQuery
        {
            BookingId = id,
            FileName = fileName
        };

        var res = await mediator.Send(command);

        if (!res.IsT0)
        {
            return res.Match(
                _ => Results.NoContent(),
                validationError => new BaseErrorResponse(validationError.Value),
                _ => new BaseNotFoundResponse(),
                _ => new BaseForbiddenResponse(),
                _ => new BaseDatabaseErrorResponse()
            );
        }
        var attachmentDto = res.AsT0;
        var file = await attachmentService.GetAttachmentAsync(attachmentDto.Value.Path);
        if (file == default)
        {
            return new BaseNotFoundResponse();
        }

        return Results.File(file, attachmentDto.Value.ContentType, attachmentDto.Value.Name);
    }
}