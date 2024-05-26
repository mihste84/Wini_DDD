namespace API.Models;

public record BaseDatabaseErrorResponse() : BaseErrorResponse(
        500,
        "Database error",
        "A database error occurred. Check the logs for details.")
{
}