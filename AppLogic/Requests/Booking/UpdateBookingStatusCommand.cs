namespace AppLogic.Requests;

public class UpdateBookingStatusCommand
    : IRequest<OneOf<Result<SqlResult>, ConflictResult, ValidationErrorResult, Error<string>, ForbiddenResult, NotFound, Unknown>>
{
    public int? BookingId { get; set; }
    public byte[]? RowVersion { get; set; }
    public WiniStatus? Status { get; set; }

    public class UpdateBookingStatusValidator : AbstractValidator<UpdateBookingStatusCommand>
    {
        public UpdateBookingStatusValidator()
        {
            RuleFor(_ => _.BookingId).NotEmpty().GreaterThan(0);
            RuleFor(_ => _.RowVersion).NotEmpty();
            RuleFor(_ => _.Status).NotEmpty();
        }
    }

    public class UpdateBookingStatusHandler(
        IBookingRepository repo,
        IMasterdataRepository masterDataRepo,
        IAuthenticationService authenticationService,
        IAuthorizationService authorizationService,
        ILogger<UpdateBookingStatusHandler> logger,
        IDateTimeService dateTimeService,
        ITransactionScopeManager transactionManager,
        IBookingValidationService validationService
    )
    : IRequestHandler<UpdateBookingStatusCommand, OneOf<Result<SqlResult>, ConflictResult, ValidationErrorResult, Error<string>, ForbiddenResult, NotFound, Unknown>>
    {
        public async Task<OneOf<Result<SqlResult>, ConflictResult, ValidationErrorResult, Error<string>, ForbiddenResult, NotFound, Unknown>> Handle(
            UpdateBookingStatusCommand request,
            CancellationToken cancellationToken)
        {
            if (!authorizationService.IsWrite())
            {
                return new ForbiddenResult();
            }

            if (!new UpdateBookingStatusValidator().IsValid(request, out var errors))
            {
                return new ValidationErrorResult(errors);
            }

            try

            {
                if (!await repo.IsSameRowVersionAsync(request.BookingId!.Value, request.RowVersion!))
                {
                    return new ConflictResult();
                }

                var result = await repo.GetBookingIdAsync(request.BookingId);
                if (result == default)
                {
                    return new NotFound();
                }

                var booking = result.Value.Booking;

                using var transaction = transactionManager.CreateTransaction();

                var res = await UpdateStatusAsync(request.Status, booking);

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
            catch (Exception ex) when (ex is DatabaseOperationException or InvalidOperationException or DatabaseValueException)
            {
                logger.LogError("An database error occurred when trying to edit booking status. Message = {message}", ex.Message);
                return new Unknown();
            }
        }

        private async Task<SqlResult?> UpdateStatusAsync(WiniStatus? status, Booking booking)
        {
            switch (status)
            {
                case WiniStatus.Saved:
                    booking.SetSavedStatus(authenticationService, authorizationService);
                    break;
                case WiniStatus.Cancelled:
                    booking.SetCancelledStatus(authenticationService);
                    break;
                case WiniStatus.SendError:
                    booking.SetSendErrorStatus(authenticationService, authorizationService);
                    break;
                case WiniStatus.ToBeSent:
                    await booking.SetToBeSentStatusAsync(
                        authenticationService,
                        authorizationService,
                        validationService,
                        await GetCompaniesAsync()
                    );
                    break;
                case WiniStatus.Sent:
                    await booking.SetSentStatusAsync(
                        authenticationService,
                        authorizationService,
                        validationService,
                        await GetCompaniesAsync()
                    );
                    break;
                case WiniStatus.ToBeAuthorized:
                    await booking.SetToBeAuthorizedStatusAsync(
                        authenticationService,
                        validationService,
                        await GetCompaniesAsync()
                    );
                    break;

                case WiniStatus.NotAuthorizedOnTime:
                    booking.SetNotAuthorizedOnTimeStatus(
                        authenticationService,
                        authorizationService,
                        dateTimeService.GetDateTimeUtc()
                    );
                    break;
            }

            var user = authenticationService.GetUserId();
            return await repo.UpdateBookingStatusAsync(booking, user);
        }

        private async Task<IEnumerable<Company>> GetCompaniesAsync()
        => await masterDataRepo.SelectAllCompaniesAsync() ?? throw new NotFoundException("Could not find any companies.");
    }
}