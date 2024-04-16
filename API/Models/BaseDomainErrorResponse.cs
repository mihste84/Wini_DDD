namespace API.Models;

public record BaseDomainErrorResponse(string Message) : BaseErrorResponse(
    422,
    "Domain error",
    Message)
{
}