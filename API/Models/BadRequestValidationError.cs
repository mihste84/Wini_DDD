using Domain.Models;

namespace API.Models;

public class BadRequestValidationError : IResult
{
    private readonly IEnumerable<ValidationError> _errors;

    public BadRequestValidationError(IEnumerable<ValidationError> errors)
    {
        _errors = errors;
    }

    public Task ExecuteAsync(HttpContext httpContext)
    {
        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status400BadRequest,
            Title = "Validation error",
            Detail = "One or more validation errors occurred.",
            Instance = httpContext.Request.Path
        };

        problemDetails.Extensions.Add("errors", _errors);
        httpContext.Response.StatusCode = 400;
        httpContext.Response.WriteAsJsonAsync(problemDetails);

        return Task.CompletedTask;
    }
}