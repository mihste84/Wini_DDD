namespace Domain.Wini.Entities;

public class BookingStatus
{
    public WiniStatus Status { get; private set; }
    public DateTime Updated { get; private set; }

    public BookingStatus(WiniStatus status, DateTime updated)
    {
        Status = status;
        Updated = updated;
    }

    public void SetStatusSaved()
    {
        if (Status is WiniStatus.Sent or WiniStatus.Saved or WiniStatus.Cancelled)
            throw new DomainLogicException(nameof(Status), WiniStatus.Saved.ToString(), "Status cannot be Sent, Saved or Cancelled");

        Status = WiniStatus.Saved;
        Updated = DateTime.UtcNow;
    }

    public void SetStatusCancelled()
    {
        if (Status is WiniStatus.Sent or WiniStatus.Cancelled)
            throw new DomainLogicException(nameof(Status), WiniStatus.Cancelled.ToString(), "Status cannot be Sent or Cancelled");

        Status = WiniStatus.Cancelled;
        Updated = DateTime.UtcNow;
    }

    public void SetStatusToBeSent()
    {
        if (Status is not WiniStatus.Saved and not WiniStatus.ToBeAuthorized)
            throw new DomainLogicException(nameof(Status), WiniStatus.ToBeSent.ToString(), "Status cannot be anything other than Saved or ToBeAuthorized");

        Status = WiniStatus.ToBeSent;
        Updated = DateTime.UtcNow;
    }

    public void SetStatusNotAuthorizedOnTime()
    {
        if (Status is not WiniStatus.ToBeAuthorized)
            throw new DomainLogicException(nameof(Status), WiniStatus.NotAuthorizedOnTime.ToString(), "Status cannot be anything other than ToBeAuthorized");

        Status = WiniStatus.NotAuthorizedOnTime;
        Updated = DateTime.UtcNow;
    }

    public void SetStatusSent()
    {
        if (Status is not WiniStatus.ToBeSent)
            throw new DomainLogicException(nameof(Status), WiniStatus.Sent.ToString(), "Status cannot be anything other than ToBeSent");

        Status = WiniStatus.Sent;
        Updated = DateTime.UtcNow;
    }
}