using System.Collections.Immutable;

namespace Domain.Wini.Entities;

public class BookingStatus
{
    public WiniStatus Status { get; private set; }
    public DateTime Updated { get; private set; }
    public User UpdatedBy { get; private set; }
    public List<BookingStatus> StatusHistory { get; } = [];

    public BookingStatus(WiniStatus status, DateTime updated, string userId)
    {
        Status = status;
        Updated = updated;
        UpdatedBy = new User(userId);
    }

    public BookingStatus(WiniStatus status, DateTime updated, string userId, List<BookingStatus> statusHistory)
    {
        Status = status;
        Updated = updated;
        UpdatedBy = new User(userId);
        StatusHistory = statusHistory;
    }

    public BookingStatus SaveStatusHistory()
    {
        var currentStatus = Copy();
        StatusHistory.Add(currentStatus);
        return currentStatus;
    }

    public void CanChangeStatusToSaved()
    {
        if (!(Status is WiniStatus.Sent or WiniStatus.Cancelled))
        {
            return;
        }

        throw new DomainLogicException(nameof(Status), WiniStatus.Saved.ToString(), "Status cannot be Sent or Cancelled");
    }

    public void CanChangeStatusToSendError()
    {
        if (Status is WiniStatus.ToBeSent)
        {
            return;
        }

        throw new DomainLogicException(nameof(Status), WiniStatus.SendError.ToString(), "Status cannot be anything other than ToBeSent");
    }

    public void CanChangeStatusToCancelled()
    {
        if (!(Status is WiniStatus.Sent or WiniStatus.Cancelled))
        {
            return;
        }

        throw new DomainLogicException(nameof(Status), WiniStatus.Cancelled.ToString(), "Status cannot be Sent or Cancelled");
    }

    public void CanChangeStatusToBeSent()
    {
        if (Status is WiniStatus.Saved or WiniStatus.ToBeAuthorized)
        {
            return;
        }

        throw new DomainLogicException(nameof(Status), WiniStatus.ToBeSent.ToString(), "Status cannot be anything other than Saved or ToBeAuthorized");
    }

    public void CanChangeStatusToNotAuthorizedOnTime()
    {
        if (Status is WiniStatus.ToBeAuthorized)
        {
            return;
        }

        throw new DomainLogicException(nameof(Status), WiniStatus.NotAuthorizedOnTime.ToString(), "Status cannot be anything other than ToBeAuthorized");
    }

    public void CanChangeStatusToBeAuthorized()
    {
        if (Status is WiniStatus.Saved)
        {
            return;
        }

        throw new DomainLogicException(nameof(Status), WiniStatus.ToBeAuthorized.ToString(), "Status cannot be anything other than Saved");
    }

    public void CanChangeStatusToSent()
    {
        if (Status is WiniStatus.ToBeSent)
        {
            return;
        }

        throw new DomainLogicException(nameof(Status), WiniStatus.Sent.ToString(), "Status cannot be anything other than ToBeSent");
    }

    public BookingStatus TryChangeStatus(WiniStatus status, string userId)
    {
        switch (status)
        {
            case WiniStatus.Saved:
                CanChangeStatusToSaved();
                break;
            case WiniStatus.SendError:
                CanChangeStatusToSendError();
                break;
            case WiniStatus.Cancelled:
                CanChangeStatusToCancelled();
                break;
            case WiniStatus.ToBeSent:
                CanChangeStatusToBeSent();
                break;
            case WiniStatus.NotAuthorizedOnTime:
                CanChangeStatusToNotAuthorizedOnTime();
                break;
            case WiniStatus.ToBeAuthorized:
                CanChangeStatusToBeAuthorized();
                break;
            case WiniStatus.Sent:
                CanChangeStatusToSent();
                break;
            default:
                throw new ArgumentException("Unknown Wini status", nameof(status));
        }

        var oldStatus = SaveStatusHistory();
        var now = DateTime.UtcNow;
        Status = status;
        Updated = now;
        UpdatedBy = new User(userId);

        return oldStatus;
    }

    public BookingStatus Copy()
    => new(
        Status,
        new DateTime(Updated.Year,
                     Updated.Month,
                     Updated.Day,
                     Updated.Hour,
                     Updated.Minute,
                     Updated.Second,
                     Updated.Millisecond,
                     DateTimeKind.Utc),
        UpdatedBy.UserId!
    );
}