namespace AppLogic.WiniLogic.Exceptions;

public class AppLogicValidationException : DomainValidationException
{
    public AppLogicValidationException(IEnumerable<ValidationFailure> errors) : base(errors)
    {
    }

    public AppLogicValidationException(IEnumerable<ValidationError> errors) : base(errors)
    {
    }

    public AppLogicValidationException(string? message, IEnumerable<ValidationError> errors) : base(message, errors)
    {
    }

    public AppLogicValidationException(string? message, IEnumerable<ValidationFailure> errors) : base(message, errors)
    {
    }

    public AppLogicValidationException(string? message, Exception? innerException, IEnumerable<ValidationFailure> errors) : base(message, innerException, errors)
    {
    }

    public AppLogicValidationException() : base()
    {
    }

    public AppLogicValidationException(string? message) : base(message)
    {
    }

    public AppLogicValidationException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}