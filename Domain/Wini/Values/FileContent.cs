namespace Domain.Wini.Values;

public record FileContent
{
    private readonly string[] _allowedContentTypes = new[] {
        "application/pdf",
        "application/vnd.ms-excel",
        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
        "application/msword",
        "text/plain",
        "image/png",
        "image/jpeg",
        "application/octet-stream"
    };
    public readonly int Size;
    public readonly string ContentType;
    public readonly string Name;
    public readonly string Path;

    public FileContent(int size, string contentType, string name, string path)
    {
        Size = size;
        ContentType = contentType;
        Name = name;
        Path = path;

        var validator = new FileContentValidator();
        var result = validator.Validate(this);
        if (!result.IsValid)
            throw new DomainValidationException(result.Errors);
    }
}