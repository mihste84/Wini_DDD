namespace API.Models;

public record BaseFormatErrorRespons(string message) : BaseErrorResponse(
    400,
    "Value format error",
    message)
{
}