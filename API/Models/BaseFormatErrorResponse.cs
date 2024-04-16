namespace API.Models;

public record BaseFormatErrorResponse(string Message) : BaseErrorResponse(
    400,
    "Value format error",
    Message)
{
}