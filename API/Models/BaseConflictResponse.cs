namespace API.Models;

public record BaseConflictResponse() : BaseErrorResponse(409, "Conflict", "A conflict occurred.")
{
}