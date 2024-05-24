namespace Domain.Wini.Values;

public readonly record struct FileContent
{
    public const int MaxAttachmentSize = 5000000; //5 MB
    public readonly long Size;
    public readonly string ContentType;
    public readonly string Name;
    public readonly string Path;

    public FileContent(long size, string contentType, string name, string path)
    {
        Size = size;
        ContentType = contentType;
        Name = name;
        Path = path;

        var validator = new FileContentValidator();
        var result = validator.Validate(this);
        if (result.IsValid)
        {
            return;
        }

        throw new DomainValidationException(result.Errors);
    }
}