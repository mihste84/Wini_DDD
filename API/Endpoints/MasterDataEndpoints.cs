namespace API.Endpoints;

public static class MasterDataEndpoints
{
    public static async Task<IResult> GetAllCompaniesAsync(IMediator mediator)
    {
        var res = await mediator.Send(new GetAllCompaniesQuery());

        return res.Match(
            success => Results.Ok(success.Value),
            _ => new BaseNotFoundResponse()
        );
    }
}