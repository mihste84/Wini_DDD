namespace Domain.Lighthouse.Entities;

public class BookingStatus
{
    public LighthouseStatus Status { get; private set; }
    public DateTime Updated { get; private set; }
    public User UpdatedBy { get; private set; }
    public List<BookingStatus> StatusHistory { get; } = [];

    public BookingStatus(LighthouseStatus status, DateTime updated, string userId)
    {
        Status = status;
        Updated = updated;
        UpdatedBy = new User(userId);
    }

    public BookingStatus(LighthouseStatus status, DateTime updated, string userId, List<BookingStatus> statusHistory)
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
        if (Status is not LighthouseStatus.Sent)
        {
            return;
        }

        throw new DomainLogicException(nameof(Status), LighthouseStatus.Saved.ToString(), "Status cannot be Sent");
    }

    public void CanChangeStatusToSendError()
    {
        if (Status is LighthouseStatus.ToBeSent)
        {
            return;
        }

        throw new DomainLogicException(nameof(Status), LighthouseStatus.SendError.ToString(), "Status cannot be anything other than ToBeSent");
    }


    public void CanChangeStatusToBeSent()
    {
        if (Status is LighthouseStatus.Saved)
        {
            return;
        }

        throw new DomainLogicException(nameof(Status), LighthouseStatus.ToBeSent.ToString(), "Status cannot be anything other than Saved");
    }


    public void CanChangeStatusToSent()
    {
        if (Status is LighthouseStatus.ToBeSent)
        {
            return;
        }

        throw new DomainLogicException(nameof(Status), LighthouseStatus.Sent.ToString(), "Status cannot be anything other than ToBeSent");
    }

    public BookingStatus TryChangeStatus(LighthouseStatus status, string userId)
    {
        switch (status)
        {
            case LighthouseStatus.Saved:
                CanChangeStatusToSaved();
                break;
            case LighthouseStatus.SendError:
                CanChangeStatusToSendError();
                break;
            case LighthouseStatus.ToBeSent:
                CanChangeStatusToBeSent();
                break;
            case LighthouseStatus.Sent:
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
        Updated,
        UpdatedBy.UserId!
    );
}