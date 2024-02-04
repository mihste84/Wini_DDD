namespace API.Endpoints;

public static class HelloEndpoints
{
    public static async Task<IResult> Hello(string msg, IMediator mediator)
    {
        var res = await mediator.Send(new HelloCommand { Input = msg });

        return res.Match(
            success => Results.Ok(new BaseResponse<string>(success.Value)),
            validationError => new BadRequestValidationError(validationError.Value)
        );
    }
}