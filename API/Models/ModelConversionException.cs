namespace API.Models;


public class ModelConversionException : Exception
{
    public ModelConversionException()
    {
    }

    public ModelConversionException(string? message) : base(message)
    {
    }

    public ModelConversionException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}