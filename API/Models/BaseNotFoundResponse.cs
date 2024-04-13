namespace API.Models;

public record BaseNotFoundResponse() : BaseErrorResponse(404, "Not found", "The requested resource was not found.")
{
}