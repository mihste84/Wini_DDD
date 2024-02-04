namespace AppLogic.Requests;

public class GetAllCompaniesQuery : IRequest<OneOf<Result<IEnumerable<CompanyDto>>, NotFound>>
{
    public class GetAllCompaniesHandler : IRequestHandler<GetAllCompaniesQuery, OneOf<Result<IEnumerable<CompanyDto>>, NotFound>>
    {
        private readonly IMasterdataRepository _companyRepo;
        private readonly ILogger<GetAllCompaniesHandler> _logger;

        public GetAllCompaniesHandler(IMasterdataRepository companyRepo, ILogger<GetAllCompaniesHandler> logger)
        {
            _companyRepo = companyRepo;
            _logger = logger;
        }

        public async Task<OneOf<Result<IEnumerable<CompanyDto>>, NotFound>> Handle(GetAllCompaniesQuery request, CancellationToken cancellationToken)
        {
            var companies = await _companyRepo.SelectAllCompaniesAsync();

            if (companies?.Any() == false)
            {
                _logger.LogWarning("No companies returned from database.");
                return new NotFound();
            }

            var dtos = companies!.Select(_ => new CompanyDto(_.Id.Value, _.CompanyCode.Code, _.Country.Code, _.Currency.Code));

            return new Result<IEnumerable<CompanyDto>>(dtos);
        }
    }
}