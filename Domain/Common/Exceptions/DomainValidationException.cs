namespace Domain.Common.Exceptions;

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

    public DomainValidationException(string? message, IEnumerable<ValidationError> errors) : base(message)
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
        Errors = [];
    }

    public DomainValidationException(string? message) : base(message)
    {
        Errors = [];
    }

    public DomainValidationException(string? message, Exception? innerException) : base(message, innerException)
    {
        Errors = [];
    }

    private static IEnumerable<ValidationError> MapFailureToValidationError(IEnumerable<ValidationFailure> errors)
        => errors.Select(_ => new ValidationError(_));
}