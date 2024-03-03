namespace Domain.Wini.Values;

public record FileContent
{
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
        if (result.IsValid)
        {
            return;
        }

        throw new DomainValidationException(result.Errors);
    }
}