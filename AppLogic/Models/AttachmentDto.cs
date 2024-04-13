namespace AppLogic.Models;

public record AttachmentDto(
    long Size,
    string ContentType,
    string Name,
    string Path
);