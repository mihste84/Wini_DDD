namespace Domain.Wini.Aggregates;

public partial class Booking
{
    public async Task SetToBeAuthorizedStatusAsync(
        IAuthenticationService authenticationService,
        IBookingValidationService validationService,
        IEnumerable<Company> companies
        )
    {
        BookingStatus.CanChangeStatusToBeAuthorized();

        var res = await validationService.ValidateAsync(this, companies);
        if (!res.IsValid)
        {
            throw new DomainValidationException($"Failed to validate booking {BookingId?.Value}.", res.Errors!);
        }

        var status = BookingStatus.TryChangeStatus(WiniStatus.ToBeAuthorized, authenticationService.GetUserId());

        AddStatusEvent(status);
    }

    public void SetCancelledStatus(IAuthenticationService authenticationService)
    {
        BookingStatus.CanChangeStatusToCancelled();

        var userId = authenticationService.GetUserId();
        if (userId != Commissioner.UserId)
        {
            throw new DomainLogicException(nameof(userId), userId, $"Only commissioners can change status to {WiniStatus.Cancelled}.");
        }

        var status = BookingStatus.TryChangeStatus(WiniStatus.Cancelled, authenticationService.GetUserId());

        AddStatusEvent(status);
    }

    public void SetSendErrorStatus(IAuthenticationService authenticationService, IAuthorizationService authorizationService)
    {
        BookingStatus.CanChangeStatusToSendError();

        if (!authorizationService.IsAdmin())
        {
            throw new DomainLogicException($"Only admins can change status to {WiniStatus.SendError}.");
        }

        RemoveAuthorizationAllRows(BookingId!.Value);
        var status = BookingStatus.TryChangeStatus(WiniStatus.SendError, authenticationService.GetUserId());

        AddStatusEvent(status);
    }

    public async Task SetSentStatusAsync(
        IAuthenticationService authenticationService,
        IAuthorizationService authorizationService,
        IBookingValidationService validationService,
        IEnumerable<Company> companies
    )
    {
        BookingStatus.CanChangeStatusToSent();

        if (!authorizationService.IsAdmin())
        {
            throw new DomainLogicException($"Only admins can change status to {WiniStatus.Sent}.");
        }

        var res = await validationService.ValidateAsync(this, companies);
        if (!res.IsValid)
        {
            throw new DomainValidationException($"Failed to validate booking {BookingId?.Value}.", res.Errors!);
        }

        var status = BookingStatus.TryChangeStatus(WiniStatus.Sent, authenticationService.GetUserId());

        AddStatusEvent(status);
    }

    public void SetNotAuthorizedOnTimeStatus(IAuthenticationService authenticationService, IAuthorizationService authorizationService, DateTime now)
    {
        BookingStatus.CanChangeStatusToNotAuthorizedOnTime();

        if (!authorizationService.IsAdmin())
        {
            throw new DomainLogicException($"Only admins can change status to {WiniStatus.NotAuthorizedOnTime}.");
        }

        if (!HaveThreeDaysPassed(now))
        {
            throw new DomainLogicException(
                nameof(now),
                now.ToString("yyyy-MM-dd HH:mm:ss"),
                $"Status cannot be changed to {WiniStatus.NotAuthorizedOnTime}. 72 hours have not passed yet.");
        }

        var status = BookingStatus.TryChangeStatus(WiniStatus.NotAuthorizedOnTime, authenticationService.GetUserId());
        AddStatusEvent(status);

        status = BookingStatus.TryChangeStatus(WiniStatus.Saved, authenticationService.GetUserId()); // Change directly to Saved. NotAutorizedOnTime is logged in history.
        AddStatusEvent(status);
    }

    public void SetSavedStatus(IAuthenticationService authenticationService, IAuthorizationService authorizationService)
    {
        BookingStatus.CanChangeStatusToSaved();

        var userId = authenticationService.GetUserId();
        if (!(userId == Commissioner.UserId || Rows.Exists(_ => _.Authorizer.UserId == userId) || authorizationService.IsAdmin()))
        {
            throw new DomainLogicException($"Only admins, commissioners or authorizers can change status to {WiniStatus.Saved}.");
        }

        RemoveAuthorizationAllRows(BookingId!.Value);

        var status = BookingStatus.TryChangeStatus(WiniStatus.Saved, authenticationService.GetUserId());

        AddStatusEvent(status);
    }

    public async Task SetToBeSentStatusAsync(
        IAuthenticationService authenticationService,
        IAuthorizationService authorizationService,
        IBookingValidationService validationService,
        IEnumerable<Company> companies)
    {
        BookingStatus.CanChangeStatusToBeSent();

        var allDebitRows = Rows.Where(_ => _.Money.IsDebitRow());
        if (allDebitRows.All(_ => _.Authorizer.HasAuthorized))
        {
            throw new DomainLogicException("All booking rows are already authorized.");
        }

        var res = await validationService.ValidateAsync(this, companies);
        if (!res.IsValid)
        {
            throw new DomainValidationException($"Failed to validate booking {BookingId?.Value}.", res.Errors!);
        }

        if (authorizationService.IsBookingAuthorizationNeeded())
        {
            var userId = authenticationService.GetUserId();
            AuthorizeRowsForUser(userId, BookingId!.Value);
            if (!allDebitRows.All(_ => _.Authorizer.HasAuthorized))
            {
                // More rows left to be authorized by other users. Just save status to history.
                BookingStatus.SaveStatusHistory();
                AddStatusEvent(BookingStatus.Copy());

                return;
            }
        }

        var status = BookingStatus.TryChangeStatus(WiniStatus.ToBeSent, authenticationService.GetUserId());

        AddStatusEvent(status);
    }

    private void AddStatusEvent(BookingStatus status) => DomainEvents.Add(new WiniStatusEvent(status));

    private bool HaveThreeDaysPassed(DateTime now)
    {
        var futureDate = BookingStatus.Updated.AddDays(3);
        return now > futureDate;
    }
}