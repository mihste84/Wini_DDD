namespace AppLogic.Requests;

public class InsertNewBookingCommand
: BookingInput, IRequest<OneOf<Result<SqlResult>, ValidationErrorResult, Error<string>, ForbiddenResult, Unknown>>
{
    public class InsertNewBookingValidator : AbstractValidator<InsertNewBookingCommand>
    {
        public InsertNewBookingValidator()
        {
            RuleFor(_ => _.BookingDate).NotEmpty();
            RuleFor(_ => _.Rows).Must(_ => _?.Length <= 1000).WithMessage("Number of rows cannot be greater than 1000.");
        }
    }

    public class InsertNewBookingHandler(
        IBookingRepository repo,
        IAuthenticationService authenticationService,
        IAuthorizationService authorizationService,
        IDateTimeService dateTimeService,
        ILogger<InsertNewBookingHandler> logger,
        ITransactionScopeManager transactionFactory
        )
        : IRequestHandler<InsertNewBookingCommand, OneOf<Result<SqlResult>, ValidationErrorResult, Error<string>, ForbiddenResult, Unknown>>
    {
        public async Task<OneOf<Result<SqlResult>, ValidationErrorResult, Error<string>, ForbiddenResult, Unknown>>
        Handle(InsertNewBookingCommand request, CancellationToken cancellationToken)
        {
            if (!authorizationService.IsWrite())
            {
                return new ForbiddenResult();
            }

            var user = authenticationService.GetUserId();
            var now = dateTimeService.GetDateTimeUtc();

            try
            {
                var booking = new Booking(
                    default,
                    new BookingStatus(WiniStatus.Saved, now, user),
                    new Commissioner(user),
                    new BookingDate(request.BookingDate ?? new DateOnly(now.Year, now.Month, now.Day)),
                    new TextToE1(request.TextToE1 ?? ""),
                    request.IsReversed ?? false,
                    new LedgerType(request.LedgerType ?? Ledgers.AA)
                );

                booking.AddMultipleRows(request.Rows, authenticationService);

                using var transaction = transactionFactory.CreateTransaction();

                var res = await repo.InsertBookingAsync(booking, user);

                transaction.Complete();

                return res != default
                    ? new Result<SqlResult>(res)
                    : new Unknown();
            }
            catch (DomainValidationException ex)
            {
                return new ValidationErrorResult(ex.Errors);
            }
            catch (Exception ex) when (ex is DomainLogicException or NotFoundException or ArgumentException or ArgumentNullException)
            {
                return new Error<string>(ex.Message);
            }
            catch (Exception ex) when (ex is DatabaseOperationException or InvalidOperationException)
            {
                logger.LogError("An database error occurred when trying to insert booking. Message = {message}", ex.Message);
                return new Unknown();
            }
        }
    }
}