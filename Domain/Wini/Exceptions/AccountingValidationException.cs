namespace Domain.Wini.Exceptions;

public class AccountingValidationException : Exception
{
    public Booking? Booking { get; }

    public AccountingValidationException() : base()
    {
    }

    public AccountingValidationException(string? message) : base(message)
    {
    }

    public AccountingValidationException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    public AccountingValidationException(string? message, Booking booking, Exception? innerException) : base(message, innerException)
    {
        Booking = booking;
    }
}