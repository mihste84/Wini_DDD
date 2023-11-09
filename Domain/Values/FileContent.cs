namespace Domain.Values;

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
        ValidateName(name);
        ValidatePath(path);
        ValidateContentType(contentType);
        ValidateSize(size);

        Size = size;
        ContentType = contentType;
        Name = name;
        Path = path;
    }

    private static void ValidateSize(int size)
    {
        if (size < 0 || size > 2000000)
        {
            throw new NumberValidationException(
                nameof(size),
                size,
                ValidationErrorCodes.OutOfRange,
                "Size must be between 0 and 2000000 byte"
            )
            { MinValue = 0, MaxValue = 2000000 };
        }
    }

    private void ValidateContentType(string contentType)
    {
        if (string.IsNullOrWhiteSpace(contentType))
        {
            throw new TextValidationException(
                nameof(contentType),
                contentType,
                ValidationErrorCodes.Required,
                "ContentType cannot be empty"
            );
        }

        if (!_allowedContentTypes.Contains(contentType))
        {
            throw new DomainLogicException(
                nameof(contentType),
                ValidationErrorCodes.IncorrectValue,
                "ContentType value is not allowed"
            );
        }
    }

    private static void ValidatePath(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            throw new TextValidationException(
                nameof(path),
                path,
                ValidationErrorCodes.Required,
                "Path cannot be empty"
            );
        }

        if (path.Length > 200)
        {
            throw new TextValidationException(
                nameof(path),
                path,
                ValidationErrorCodes.TextTooLong,
                "Path cannot be longer than 200 characters"
            )
            { MaxLength = 200 };
        }
    }

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new TextValidationException(
                nameof(name),
                name,
                ValidationErrorCodes.Required,
                "Name cannot be empty"
            );
        }

        if (name.Length > 100)
        {
            throw new TextValidationException(
                nameof(name),
                name,
                ValidationErrorCodes.TextTooLong,
                "Name cannot be longer than 100 characters"
            )
            { MaxLength = 100 };
        }
    }
}