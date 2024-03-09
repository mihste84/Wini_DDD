namespace AppLogic.Models;

public record UploadedAttachmentInput(
    Stream? Content,
    string? FileName,
    long? Length,
    string? ContentType
);