
namespace AppLogic.WiniLogic.Requests;


public class SearchBookingsQuery
: DynamicSearchQuery, IRequest<OneOf<Result<SearchResultWrapper<BookingSearchResult>>, ForbiddenResult, ValidationErrorResult>>
{
    public class SearchBookingsValidator : AbstractValidator<SearchBookingsQuery>
    {
        public SearchBookingsValidator()
        {
            RuleFor(_ => _.EndRow).GreaterThan(0).NotEmpty();
            RuleFor(_ => _.StartRow).NotNull();
            RuleFor(_ => _.SearchItems).NotNull();
            RuleFor(_ => _.OrderBy).NotEmpty().MaximumLength(50);
            RuleFor(_ => _.OrderByDirection).NotEmpty().MaximumLength(4).Matches("ASC|DESC");
        }
    }

    public class SearchBookingsHandler(
        IBookingRepository repo,
        IAuthorizationService authorizationService)
    : IRequestHandler<SearchBookingsQuery, OneOf<Result<SearchResultWrapper<BookingSearchResult>>, ForbiddenResult, ValidationErrorResult>>
    {
        public async Task<OneOf<Result<SearchResultWrapper<BookingSearchResult>>, ForbiddenResult, ValidationErrorResult>> Handle(
            SearchBookingsQuery request,
            CancellationToken cancellationToken
        )
        {
            if (!authorizationService.IsRead())
            {
                return new ForbiddenResult();
            }

            if (!new SearchBookingsValidator().IsValid(request, out var errors))
            {
                return new ValidationErrorResult(errors);
            }

            var result = await repo.SearchBookingsAsync(request);
            var itemsPerPage = request.EndRow - request.StartRow;
            var hasMorePages = result.Length >= itemsPerPage;
            var dto = new SearchResultWrapper<BookingSearchResult>(
                HasMorePages: hasMorePages,
                Items: result,
                NextStartRow: hasMorePages ? request.StartRow + itemsPerPage : request.StartRow,
                NextEndRow: hasMorePages ? request.EndRow + itemsPerPage : request.EndRow
            );

            return new Result<SearchResultWrapper<BookingSearchResult>>(dto);
        }
    }
}