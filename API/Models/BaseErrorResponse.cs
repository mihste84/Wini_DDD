namespace API.Models;

public record BaseErrorResponse : IResult
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
        int statusCode = 422,
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
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1", // TODO: Change this to a link to your error documentation
            Status = _statusCode,
            Title = _title,
            Detail = _detail,
            Instance = httpContext.Request.Path,
        };

        if (_errors?.Any() == true)
        {
            problemDetails.Extensions.Add("errors", _errors);
        }

        httpContext.Response.StatusCode = _statusCode;
        httpContext.Response.WriteAsJsonAsync(problemDetails);

        return Task.CompletedTask;
    }
}