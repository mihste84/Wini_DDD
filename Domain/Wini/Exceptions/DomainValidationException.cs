namespace Domain.Wini.Exceptions;

public class DomainValidationException : Exception
{
    public readonly IEnumerable<ValidationError> Errors;

    public DomainValidationException(IEnumerable<ValidationFailure> errors) : base()
    {
        Errors = MapFailureToValidationError(errors);
    }

    public DomainValidationException(IEnumerable<ValidationError> errors) : base()
    {
        Errors = errors;
    }

    public DomainValidationException(string? message, IEnumerable<ValidationFailure> errors) : base(message)
    {
        Errors = MapFailureToValidationError(errors);
    }

    public DomainValidationException(string? message, Exception? innerException, IEnumerable<ValidationFailure> errors) : base(message, innerException)
    {
        Errors = MapFailureToValidationError(errors);
    }

    public DomainValidationException() : base()
    {
        Errors = Array.Empty<ValidationError>();
    }

    public DomainValidationException(string? message) : base(message)
    {
        Errors = Array.Empty<ValidationError>();
    }

    public DomainValidationException(string? message, Exception? innerException) : base(message, innerException)
    {
        Errors = Array.Empty<ValidationError>();
    }

    private static IEnumerable<ValidationError> MapFailureToValidationError(IEnumerable<ValidationFailure> errors)
        => errors.Select(_ => new ValidationError(_));
}