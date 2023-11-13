namespace Domain.Validators;

public class FileContentValidator : AbstractValidator<FileContent>
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
    public FileContentValidator()
    {
        RuleFor(_ => _.ContentType)
            .NotEmpty()
            .Must(_ => _allowedContentTypes.Contains(_)).WithMessage(_ => $"Content type value '{_}' is not allowed.")
            .WithName("Content Type");

        RuleFor(_ => _.Name).NotEmpty().MaximumLength(100).WithName("Filename");
        RuleFor(_ => _.Path).NotEmpty().MaximumLength(200).WithName("File Path");
        RuleFor(_ => _.Size)
            .InclusiveBetween(0, 2000000)
            .WithMessage("Size must be between 0 and 2000000 byte")
            .WithName("File Size");
    }
}