namespace AppLogic.Requests;

public class GetBookingByIdQuery : IRequest<OneOf<Result<BookingDto>, ValidationErrorResult, ForbiddenResult, NotFound>>
{
    public int? BookingId { get; set; }

    public class GetBookingByIdValidator : AbstractValidator<GetBookingByIdQuery>
    {
        public GetBookingByIdValidator()
        {
            RuleFor(_ => _.BookingId).NotEmpty().GreaterThan(0);
        }
    }

    public class GetBookingByIdHandler : IRequestHandler<GetBookingByIdQuery, OneOf<Result<BookingDto>, ValidationErrorResult, ForbiddenResult, NotFound>>
    {
        private readonly IBookingRepository _repo;
        private readonly IAuthorizationService _authorizationService;

        public GetBookingByIdHandler(IBookingRepository repo, IAuthorizationService authorizationService)
        {
            _repo = repo;
            _authorizationService = authorizationService;
        }

        public async Task<OneOf<Result<BookingDto>, ValidationErrorResult, ForbiddenResult, NotFound>> Handle(GetBookingByIdQuery request, CancellationToken cancellationToken)
        {
            if (!_authorizationService.IsRead())
                return new ForbiddenResult();

            if (!new GetBookingByIdValidator().IsValid(request, out var errors))
                return new ValidationErrorResult(errors);

            var result = await _repo.GetBookingIdAsync(request.BookingId);
            if (result == default) return new NotFound();

            var booking = result.Value.Booking;

            var dto = new BookingDto(
                (short)booking.BookingStatus.Status,
                booking.Header.BookingDate.Date,
                booking.Header.TextToE1.Text,
                booking.Header.IsReversed,
                (short)booking.Header.LedgerType.Type,
                booking.BookingStatus.UpdatedBy.UserId!,
                booking.BookingStatus.Updated,
                result.Value.RowVersion,
                booking.Rows.Select(_ => new BookingRowDto(
                    _.RowNumber,
                    _.BusinessUnit.ToString(),
                    _.Account.Value,
                    _.Account.Subsidiary,
                    _.Subledger.Value,
                    _.Subledger.Type,
                    _.CostObject1.Value,
                    _.CostObject1.Type,
                    _.CostObject2.Value,
                    _.CostObject2.Type,
                    _.CostObject3.Value,
                    _.CostObject3.Type,
                    _.CostObject4.Value,
                    _.CostObject4.Type,
                    _.Remark.Text,
                    _.Authorizer.UserId,
                    _.Authorizer.HasAuthorized,
                    _.Money.Amount,
                    _.Money.Currency.CurrencyCode.Code,
                    _.Money.Currency.ExchangeRate
                ))
            );
            return new Result<BookingDto>(dto);
        }
    }
}