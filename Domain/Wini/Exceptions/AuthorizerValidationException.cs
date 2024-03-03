namespace Domain.Wini.Exceptions;

public class AuthorizerValidationException : Exception
{
    public Booking? Booking { get; }

    public AuthorizerValidationException() : base()
    {
    }

    public AuthorizerValidationException(string? message) : base(message)
    {
    }

    public AuthorizerValidationException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    public AuthorizerValidationException(string? message, Booking booking, Exception? innerException) : base(message, innerException)
    {
        Booking = booking;
    }
}