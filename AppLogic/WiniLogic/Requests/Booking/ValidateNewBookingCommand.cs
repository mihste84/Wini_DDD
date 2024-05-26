namespace AppLogic.WiniLogic.Requests;

public class ValidateNewBookingCommand
: BookingInput, IRequest<OneOf<Result<BookingValidationResultModel>, ForbiddenResult, Unknown, Error<string>>>
{
    public class ValidateNewBookingHandler(
        IBookingValidationService bookingValidationService,
        IMasterdataRepository masterdataRepository,
        IAuthorizationService authorizationService,
        IAuthenticationService authenticationService,
        ILogger<ValidateNewBookingHandler> logger
    )
    : IRequestHandler<ValidateNewBookingCommand, OneOf<Result<BookingValidationResultModel>, ForbiddenResult, Unknown, Error<string>>>
    {
        public async Task<OneOf<Result<BookingValidationResultModel>, ForbiddenResult, Unknown, Error<string>>> Handle(
            ValidateNewBookingCommand request,
            CancellationToken cancellationToken
        )
        {
            if (!authorizationService.IsWrite())
            {
                return new ForbiddenResult();
            }

            try
            {
                var user = authenticationService.GetUserId();
                var booking = new Booking(
                    default,
                    new BookingStatus(WiniStatus.New, DateTime.UtcNow, user),
                    new Commissioner(user),
                    new BookingDate(request.BookingDate.GetValueOrDefault()),
                    new TextToE1(request.TextToE1),
                    request.IsReversed.GetValueOrDefault(),
                    new LedgerType(request.LedgerType.GetValueOrDefault()),
                    request.Rows?.Select(MapFromModel).ToList() ?? [],
                    [],
                    [],
                    [],
                    DateTime.UtcNow
                );
                var companies = await masterdataRepository.SelectAllCompaniesAsync();
                var result = await bookingValidationService.ValidateAsync(booking, companies);

                return new Result<BookingValidationResultModel>(result);
            }
            catch (AccountingValidationException ex)
            {
                logger.LogError(ex, "Failed to validate with accounting system.");
                return new Unknown();
            }
            catch (AuthorizerValidationException ex)
            {
                logger.LogError(ex, "Failed to validate with authorizer system.");
                return new Unknown();
            }
            catch (DomainValidationException ex)
            { // Catch validation errors when booking is created
                var result = new BookingValidationResultModel
                {
                    Message = "Booking values are not valid.",
                    IsValid = false,
                    Errors = ex.Errors
                };

                return new Result<BookingValidationResultModel>(result);
            }
            catch (FormatException ex)
            {
                logger.LogError(ex, "Failed to parse booking values.");
                return new Error<string>(ex.Message);
            }
        }

        private BookingRow MapFromModel(BookingRowModel model)
        => new(
            model.RowNumber,
            new BusinessUnit(model.BusinessUnit),
            new Account(model.Account, model.Subsidiary),
            new Subledger(model.Subledger, model.SubledgerType),
            new CostObject(1, model.CostObject1, model.CostObjectType1),
            new CostObject(2, model.CostObject2, model.CostObjectType2),
            new CostObject(3, model.CostObject3, model.CostObjectType3),
            new CostObject(4, model.CostObject4, model.CostObjectType4),
            new Remark(model.Remark),
            new Authorizer(model.Authorizer, false),
            new Money(model.Amount, model.CurrencyCode, model.ExchangeRate)
        );
    }
}
