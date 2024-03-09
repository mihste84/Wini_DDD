namespace AppLogic.Requests;

public class GetAllCompaniesQuery : IRequest<OneOf<Result<IEnumerable<CompanyDto>>, NotFound>>
{
    public class GetAllCompaniesHandler(
        IMasterdataRepository companyRepo,
        ILogger<GetAllCompaniesHandler> logger) : IRequestHandler<GetAllCompaniesQuery, OneOf<Result<IEnumerable<CompanyDto>>, NotFound>>
    {
        public async Task<OneOf<Result<IEnumerable<CompanyDto>>, NotFound>> Handle(GetAllCompaniesQuery request, CancellationToken cancellationToken)
        {
            var companies = await companyRepo.SelectAllCompaniesAsync();

            if (companies?.Any() == false)
            {
                logger.LogWarning("No companies returned from database.");
                return new NotFound();
            }

            var dtos = companies!.Select(_ => new CompanyDto(_.Id.Value, _.CompanyCode.Code, _.Country.Code, _.Currency.Code));

            return new Result<IEnumerable<CompanyDto>>(dtos);
        }
    }
}