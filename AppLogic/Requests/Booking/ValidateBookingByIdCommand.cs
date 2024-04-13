namespace AppLogic.Requests;

public class ValidateBookingByIdCommand
: IRequest<OneOf<Result<BookingValidationResultModel>, NotFound, ForbiddenResult, Unknown, Error<string>>>
{
    public int? Id { get; set; }
    public class ValidateBookingByIdHandler(
        IBookingValidationService bookingValidationService,
        IMasterdataRepository masterdataRepository,
        IBookingRepository bookingRepository,
        IAuthorizationService authorizationService,
        ILogger<ValidateBookingByIdHandler> logger
    )
    : IRequestHandler<ValidateBookingByIdCommand, OneOf<Result<BookingValidationResultModel>, NotFound, ForbiddenResult, Unknown, Error<string>>>
    {
        public async Task<OneOf<Result<BookingValidationResultModel>, NotFound, ForbiddenResult, Unknown, Error<string>>> Handle(
            ValidateBookingByIdCommand request,
            CancellationToken cancellationToken
        )
        {
            if (!authorizationService.IsWrite())
            {
                return new ForbiddenResult();
            }

            try
            {
                var bookingRes = await bookingRepository.GetBookingIdAsync(request.Id!.Value, true);
                if (bookingRes == default)
                {
                    return new NotFound();
                }
                var companies = await masterdataRepository.SelectAllCompaniesAsync();
                var result = await bookingValidationService.ValidateAsync(bookingRes.Value.Booking, companies);

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
    }
}
