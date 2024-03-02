namespace AppLogic.Requests;

public class DeleteBookingByIdCommand : IRequest<OneOf<Success, ValidationErrorResult, Error<string>, ForbiddenResult, NotFound, Unknown>>
{
    public int? Id { get; set; }
    public class DeleteBookingByIdValidator : AbstractValidator<DeleteBookingByIdCommand>
    {
        public DeleteBookingByIdValidator()
        {
            RuleFor(_ => _.Id).NotEmpty().GreaterThan(0);
        }
    }

    public class DeleteBookingByIdHandler
    : IRequestHandler<DeleteBookingByIdCommand, OneOf<Success, ValidationErrorResult, Error<string>, ForbiddenResult, NotFound, Unknown>>
    {
        private readonly IBookingRepository _repo;
        private readonly IAuthenticationService _authenticationService;
        private readonly IAuthorizationService _authorizationService;
        private readonly ILogger<DeleteBookingByIdHandler> _logger;

        public DeleteBookingByIdHandler(
            IBookingRepository repo,
            IAuthenticationService authenticationService,
            IAuthorizationService authorizationService,
            ILogger<DeleteBookingByIdHandler> logger
        )
        {
            _repo = repo;
            _authenticationService = authenticationService;
            _authorizationService = authorizationService;
            _logger = logger;
        }

        public async Task<OneOf<Success, ValidationErrorResult, Error<string>, ForbiddenResult, NotFound, Unknown>> Handle(DeleteBookingByIdCommand request, CancellationToken cancellationToken)
        {
            if (!_authorizationService.IsWrite())
                return new ForbiddenResult();

            if (!new DeleteBookingByIdValidator().IsValid(request, out var errors))
                return new ValidationErrorResult(errors);

            try
            {
                var result = await _repo.GetBookingIdAsync(request.Id);
                if (result == default) return new NotFound();
                var booking = result.Value.Booking;

                var (canDelete, reason) = booking.CanDeleteBooking(_authenticationService, _authorizationService);
                if (!canDelete)
                    return reason == "Forbidden" ? new ForbiddenResult() : new Error<string>("Cannot delete booking when status is Sent.");

                await _repo.DeleteBookingIdAsync(request.Id);

                return new Success();
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
                _logger.LogError("An database error occurred when trying to delete booking. Message = {message}", ex.Message);
                return new Unknown();
            }
            throw new NotImplementedException();
        }
    }
}
