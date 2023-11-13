namespace Tests.UnitTests.Wini;

public class WiniStatusTests
{
    [Fact]
    public void Create_Wini_Status()
    {
        var status = new BookingStatus(WiniStatus.Saved, DateTime.UtcNow);
        Assert.Equal(WiniStatus.Saved, status.Status);
        Assert.Equal(DateTime.UtcNow.Date, status.Updated.Date);
    }

    [Theory]
    [InlineData(WiniStatus.NotAuthorizedOnTime)]
    [InlineData(WiniStatus.ToBeAuthorized)]
    [InlineData(WiniStatus.ToBeSent)]
    [InlineData(WiniStatus.SendError)]
    public void Set_Wini_Status_Saved(WiniStatus status)
    {
        var bookingStatus = new BookingStatus(status, DateTime.UtcNow);
        bookingStatus.SetStatusSaved();

        Assert.Equal(WiniStatus.Saved, bookingStatus.Status);
        Assert.Equal(DateTime.UtcNow.Date, bookingStatus.Updated.Date);
    }

    [Theory]
    [InlineData(WiniStatus.Sent)]
    [InlineData(WiniStatus.Saved)]
    [InlineData(WiniStatus.Cancelled)]
    public void Fail_To_Set_Wini_Status_Saved(WiniStatus status)
    {
        var bookingStatus = new BookingStatus(status, DateTime.UtcNow);

        var ex = Assert.Throws<DomainLogicException>(() => bookingStatus.SetStatusSaved());

        Assert.Equal("Status cannot be Sent, Saved or Cancelled", ex.Message);
        Assert.Equal(WiniStatus.Saved.ToString(), ex.AttemptedValue);
        Assert.Equal("Status", ex.PropertyName);
    }

    [Theory]
    [InlineData(WiniStatus.NotAuthorizedOnTime)]
    [InlineData(WiniStatus.ToBeAuthorized)]
    [InlineData(WiniStatus.ToBeSent)]
    [InlineData(WiniStatus.SendError)]
    [InlineData(WiniStatus.Saved)]
    public void Set_Wini_Status_Cancelled(WiniStatus status)
    {
        var bookingStatus = new BookingStatus(status, DateTime.UtcNow);

        bookingStatus.SetStatusCancelled();

        Assert.Equal(WiniStatus.Cancelled, bookingStatus.Status);
        Assert.Equal(DateTime.UtcNow.Date, bookingStatus.Updated.Date);
    }

    [Theory]
    [InlineData(WiniStatus.Sent)]
    [InlineData(WiniStatus.Cancelled)]
    public void Fail_To_Set_Wini_Status_Cancelled(WiniStatus status)
    {
        var bookingStatus = new BookingStatus(status, DateTime.UtcNow);

        var ex = Assert.Throws<DomainLogicException>(() => bookingStatus.SetStatusCancelled());

        Assert.Equal("Status cannot be Sent or Cancelled", ex.Message);
        Assert.Equal(WiniStatus.Cancelled.ToString(), ex.AttemptedValue);
        Assert.Equal("Status", ex.PropertyName);
    }

    [Theory]
    [InlineData(WiniStatus.Saved)]
    [InlineData(WiniStatus.ToBeAuthorized)]
    public void Set_Wini_Status_ToBeSent(WiniStatus status)
    {
        var bookingStatus = new BookingStatus(status, DateTime.UtcNow);

        bookingStatus.SetStatusToBeSent();

        Assert.Equal(WiniStatus.ToBeSent, bookingStatus.Status);
        Assert.Equal(DateTime.UtcNow.Date, bookingStatus.Updated.Date);
    }

    [Theory]
    [InlineData(WiniStatus.Cancelled)]
    [InlineData(WiniStatus.Sent)]
    [InlineData(WiniStatus.SendError)]
    [InlineData(WiniStatus.NotAuthorizedOnTime)]
    [InlineData(WiniStatus.ToBeSent)]
    public void Fail_To_Set_Wini_Status_ToBeSent(WiniStatus status)
    {
        var bookingStatus = new BookingStatus(status, DateTime.UtcNow);

        var ex = Assert.Throws<DomainLogicException>(() => bookingStatus.SetStatusToBeSent());

        Assert.Equal("Status cannot be anything other than Saved or ToBeAuthorized", ex.Message);
        Assert.Equal(WiniStatus.ToBeSent.ToString(), ex.AttemptedValue);
        Assert.Equal("Status", ex.PropertyName);
    }

    [Fact]
    public void Set_Wini_Status_NotAuthorizedOnTime()
    {
        var status = new BookingStatus(WiniStatus.ToBeAuthorized, DateTime.UtcNow);

        status.SetStatusNotAuthorizedOnTime();

        Assert.Equal(WiniStatus.NotAuthorizedOnTime, status.Status);
        Assert.Equal(DateTime.UtcNow.Date, status.Updated.Date);
    }

    [Theory]
    [InlineData(WiniStatus.Cancelled)]
    [InlineData(WiniStatus.Sent)]
    [InlineData(WiniStatus.SendError)]
    [InlineData(WiniStatus.NotAuthorizedOnTime)]
    [InlineData(WiniStatus.Saved)]
    [InlineData(WiniStatus.ToBeSent)]
    public void Fail_To_Set_Wini_Status_NotAuthorizedOnTime(WiniStatus status)
    {
        var bookingStatus = new BookingStatus(status, DateTime.UtcNow);

        var ex = Assert.Throws<DomainLogicException>(() => bookingStatus.SetStatusNotAuthorizedOnTime());

        Assert.Equal("Status cannot be anything other than ToBeAuthorized", ex.Message);
        Assert.Equal(WiniStatus.NotAuthorizedOnTime.ToString(), ex.AttemptedValue);
        Assert.Equal("Status", ex.PropertyName);
    }

    [Fact]
    public void Set_Wini_Status_Sent()
    {
        var status = new BookingStatus(WiniStatus.ToBeSent, DateTime.UtcNow);

        status.SetStatusSent();

        Assert.Equal(WiniStatus.Sent, status.Status);
        Assert.Equal(DateTime.UtcNow.Date, status.Updated.Date);
    }

    [Theory]
    [InlineData(WiniStatus.Cancelled)]
    [InlineData(WiniStatus.Sent)]
    [InlineData(WiniStatus.SendError)]
    [InlineData(WiniStatus.NotAuthorizedOnTime)]
    [InlineData(WiniStatus.Saved)]
    [InlineData(WiniStatus.ToBeAuthorized)]
    public void Fail_To_Set_Wini_Status_Sent(WiniStatus status)
    {
        var bookingStatus = new BookingStatus(status, DateTime.UtcNow);

        var ex = Assert.Throws<DomainLogicException>(() => bookingStatus.SetStatusSent());

        Assert.Equal("Status cannot be anything other than ToBeSent", ex.Message);
        Assert.Equal(WiniStatus.Sent.ToString(), ex.AttemptedValue);
        Assert.Equal("Status", ex.PropertyName);
    }
}