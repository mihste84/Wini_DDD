using Domain.Models;

namespace API.Models;

public class BaseErrorResponse : IResult
{
    private readonly int _statusCode;
    private readonly string _title;
    private readonly string _detail;
    private readonly IEnumerable<ValidationError>? _errors;

    public BaseErrorResponse(int statusCode, string title, string detail)
    {
        _statusCode = statusCode;
        _title = title;
        _detail = detail;
    }

    public BaseErrorResponse(
        IEnumerable<ValidationError> errors,
        int statusCode = 400,
        string title = "Validation error",
        string detail = "One or more validation errors occurred."
    )
    {
        _statusCode = statusCode;
        _title = title;
        _detail = detail;
        _errors = errors;
    }

    public Task ExecuteAsync(HttpContext httpContext)
    {
        var problemDetails = new ProblemDetails
        {
            Status = _statusCode,
            Title = _title,
            Detail = _detail,
            Instance = httpContext.Request.Path
        };

        if (_errors?.Any() == true) problemDetails.Extensions.Add("errors", _errors);
        httpContext.Response.StatusCode = _statusCode;
        httpContext.Response.WriteAsJsonAsync(problemDetails);

        return Task.CompletedTask;
    }
}