namespace AppLogic.Models;

public record UploadedAttachmentInput(
    string? FileName,
    long? Length,
    string? ContentType,
    string? Path
);