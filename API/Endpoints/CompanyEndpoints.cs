namespace API.Endpoints;

public static class CompanyEndpoints
{
    public static async Task<IResult> GetAllCompanies(IMediator mediator)
    {
        var res = await mediator.Send(new GetAllCompaniesQuery());

        return res.Match(
            success => Results.Ok(new BaseResponse<IEnumerable<CompanyDto>>(success.Value)),
            notFound => Results.NotFound()
        );
    }
}