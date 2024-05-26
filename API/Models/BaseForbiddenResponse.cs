namespace API.Models;

public record BaseForbiddenResponse() : BaseErrorResponse(403, "Forbidden", "You are not authorized to access this resource.")
{
}