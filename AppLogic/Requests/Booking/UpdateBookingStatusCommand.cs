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

    public class UpdateBookingStatusHandler
        : IRequestHandler<UpdateBookingStatusCommand, OneOf<Result<SqlResult>, ConflictResult, ValidationErrorResult, Error<string>, ForbiddenResult, NotFound, Unknown>>
    {
        private readonly IBookingRepository _repo;
        private readonly IAuthenticationService _authenticationService;
        private readonly IAuthorizationService _authorizationService;
        private readonly IBookingValidationService _validationService;
        private readonly IMasterdataRepository _masterDataRepo;
        private readonly IDateTimeService _dateTimeService;
        private readonly ITransactionScopeManager _transactionManager;
        private readonly ILogger<UpdateBookingStatusHandler> _logger;

        public UpdateBookingStatusHandler(
            IBookingRepository repo,
            IMasterdataRepository masterDataRepo,
            IAuthenticationService authenticationService,
            IAuthorizationService authorizationService,
            ILogger<UpdateBookingStatusHandler> logger,
            IDateTimeService dateTimeService,
            ITransactionScopeManager transactionManager,
            IBookingValidationService validationService
        )
        {
            _repo = repo;
            _masterDataRepo = masterDataRepo;
            _authenticationService = authenticationService;
            _authorizationService = authorizationService;
            _logger = logger;
            _validationService = validationService;
            _transactionManager = transactionManager;
            _dateTimeService = dateTimeService;
        }

        public async Task<OneOf<Result<SqlResult>, ConflictResult, ValidationErrorResult, Error<string>, ForbiddenResult, NotFound, Unknown>> Handle(UpdateBookingStatusCommand request, CancellationToken cancellationToken)
        {
            if (!_authorizationService.IsWrite())
                return new ForbiddenResult();

            if (!new UpdateBookingStatusValidator().IsValid(request, out var errors))
                return new ValidationErrorResult(errors);
            try

            {
                if (!await _repo.IsSameRowVersionAsync(request.BookingId!.Value, request.RowVersion!))
                    return new ConflictResult();

                var result = await _repo.GetBookingIdAsync(request.BookingId);
                if (result == default) return new NotFound();
                var booking = result.Value.Booking;

                using var transaction = _transactionManager.CreateTransaction();

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
            catch (Exception ex) when (ex is DomainLogicException or NotFoundException)
            {
                return new Error<string>(ex.Message);
            }
            catch (Exception ex) when (ex is DatabaseOperationException or InvalidOperationException or DatabaseValueException)
            {
                _logger.LogError("An database error occurred when trying to edit booking status. Message = {message}", ex.Message);
                return new Unknown();
            }
        }

        private async Task<SqlResult?> UpdateStatusAsync(WiniStatus? status, Booking booking)
        {
            switch (status)
            {
                case WiniStatus.Saved:
                    booking.SetSavedStatus(_authenticationService, _authorizationService); break;
                case WiniStatus.Cancelled:
                    booking.SetCancelledStatus(_authenticationService); break;
                case WiniStatus.SendError:
                    booking.SetSendErrorStatus(_authenticationService, _authorizationService); break;
                case WiniStatus.ToBeSent:
                    await booking.SetToBeSentStatusAsync(
                        _authenticationService,
                        _authorizationService,
                        _validationService,
                        await GetCompaniesAsync()
                    );
                    break;
                case WiniStatus.Sent:
                    await booking.SetSentStatusAsync(
                        _authenticationService,
                        _authorizationService,
                        _validationService,
                        await GetCompaniesAsync()
                    );
                    break;
                case WiniStatus.ToBeAuthorized:
                    await booking.SetToBeAuthorizedStatusAsync(
                        _authenticationService,
                        _validationService,
                        await GetCompaniesAsync()
                    );
                    break;

                case WiniStatus.NotAuthorizedOnTime:
                    booking.SetNotAuthorizedOnTimeStatus(
                        _authenticationService,
                        _authorizationService,
                        _dateTimeService.GetDateTimeUtc()
                    );
                    break;
            }
            var user = _authenticationService.GetUserId();
            return await _repo.UpdateBookingStatusAsync(booking, user);
        }

        private async Task<IEnumerable<Company>> GetCompaniesAsync()
        => await _masterDataRepo.SelectAllCompaniesAsync() ?? throw new NotFoundException("Could not find any companies.");
    }
}