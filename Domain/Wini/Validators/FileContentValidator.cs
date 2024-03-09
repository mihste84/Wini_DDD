namespace Domain.Wini.Validators;

public class FileContentValidator : AbstractValidator<FileContent>
{
    private readonly string[] _allowedContentTypes = [
        "application/pdf",
        "application/vnd.ms-excel",
        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
        "application/msword",
        "text/plain",
        "image/png",
        "image/jpeg",
        "application/octet-stream"
    ];

    public FileContentValidator()
    {
        RuleFor(_ => _.ContentType)
            .MaximumLength(200)
            .Must(_ => _allowedContentTypes.Contains(_))
            .WithMessage(_ => $"Content type value '{_.ContentType}' is not allowed.")
            .WithName("Content Type");

        RuleFor(_ => _.Name).NotEmpty().MaximumLength(300).WithName("Filename");
        RuleFor(_ => _.Path).NotEmpty().MaximumLength(400).WithName("File Path");
        RuleFor(_ => _.Size)
            .InclusiveBetween(0, FileContent.MaxAttachmentSize)
            .WithMessage($"Size must be between 0 and {FileContent.MaxAttachmentSize} byte.")
            .WithName("File Size");
    }
}