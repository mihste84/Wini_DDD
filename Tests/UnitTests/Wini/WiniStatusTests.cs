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
        bookingStatus.TryChangeStatus(WiniStatus.Saved);

        Assert.Equal(WiniStatus.Saved, bookingStatus.Status);
        Assert.Equal(DateTime.UtcNow.Date, bookingStatus.Updated.Date);
        Assert.Single(bookingStatus.StatusHistory);
    }

    [Theory]
    [InlineData(WiniStatus.Sent)]
    [InlineData(WiniStatus.Saved)]
    [InlineData(WiniStatus.Cancelled)]
    public void Fail_To_Set_Wini_Status_Saved(WiniStatus status)
    {
        var bookingStatus = new BookingStatus(status, DateTime.UtcNow);

        var ex = Assert.Throws<DomainLogicException>(() => bookingStatus.TryChangeStatus(WiniStatus.Saved));

        Assert.Equal("Status cannot be Sent, Saved or Cancelled", ex.Message);
        Assert.Equal(WiniStatus.Saved.ToString(), ex.AttemptedValue);
        Assert.Equal("Status", ex.PropertyName);
        Assert.Empty(bookingStatus.StatusHistory);
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

        bookingStatus.TryChangeStatus(WiniStatus.Cancelled);

        Assert.Equal(WiniStatus.Cancelled, bookingStatus.Status);
        Assert.Equal(DateTime.UtcNow.Date, bookingStatus.Updated.Date);
        Assert.Single(bookingStatus.StatusHistory);
    }

    [Theory]
    [InlineData(WiniStatus.Sent)]
    [InlineData(WiniStatus.Cancelled)]
    public void Fail_To_Set_Wini_Status_Cancelled(WiniStatus status)
    {
        var bookingStatus = new BookingStatus(status, DateTime.UtcNow);

        var ex = Assert.Throws<DomainLogicException>(() => bookingStatus.TryChangeStatus(WiniStatus.Cancelled));

        Assert.Equal("Status cannot be Sent or Cancelled", ex.Message);
        Assert.Equal(WiniStatus.Cancelled.ToString(), ex.AttemptedValue);
        Assert.Equal("Status", ex.PropertyName);
        Assert.Empty(bookingStatus.StatusHistory);
    }

    [Theory]
    [InlineData(WiniStatus.Saved)]
    [InlineData(WiniStatus.ToBeAuthorized)]
    public void Set_Wini_Status_ToBeSent(WiniStatus status)
    {
        var bookingStatus = new BookingStatus(status, DateTime.UtcNow);

        bookingStatus.TryChangeStatus(WiniStatus.ToBeSent);

        Assert.Equal(WiniStatus.ToBeSent, bookingStatus.Status);
        Assert.Equal(DateTime.UtcNow.Date, bookingStatus.Updated.Date);
        Assert.Single(bookingStatus.StatusHistory);
    }

    [Theory]
    [InlineData(WiniStatus.ToBeSent)]
    public void Set_Wini_Status_SendError(WiniStatus status)
    {
        var bookingStatus = new BookingStatus(status, DateTime.UtcNow);

        bookingStatus.TryChangeStatus(WiniStatus.SendError);

        Assert.Equal(WiniStatus.SendError, bookingStatus.Status);
        Assert.Equal(DateTime.UtcNow.Date, bookingStatus.Updated.Date);
        Assert.Single(bookingStatus.StatusHistory);
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

        var ex = Assert.Throws<DomainLogicException>(() => bookingStatus.TryChangeStatus(WiniStatus.ToBeSent));

        Assert.Equal("Status cannot be anything other than Saved or ToBeAuthorized", ex.Message);
        Assert.Equal(WiniStatus.ToBeSent.ToString(), ex.AttemptedValue);
        Assert.Equal("Status", ex.PropertyName);
        Assert.Empty(bookingStatus.StatusHistory);
    }

    [Fact]
    public void Set_Wini_Status_NotAuthorizedOnTime()
    {
        var status = new BookingStatus(WiniStatus.ToBeAuthorized, DateTime.UtcNow);

        status.TryChangeStatus(WiniStatus.NotAuthorizedOnTime);

        Assert.Equal(WiniStatus.NotAuthorizedOnTime, status.Status);
        Assert.Equal(DateTime.UtcNow.Date, status.Updated.Date);
        Assert.Single(status.StatusHistory);
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

        var ex = Assert.Throws<DomainLogicException>(() => bookingStatus.TryChangeStatus(WiniStatus.NotAuthorizedOnTime));

        Assert.Equal("Status cannot be anything other than ToBeAuthorized", ex.Message);
        Assert.Equal(WiniStatus.NotAuthorizedOnTime.ToString(), ex.AttemptedValue);
        Assert.Equal("Status", ex.PropertyName);
        Assert.Empty(bookingStatus.StatusHistory);
    }

    [Theory]
    [InlineData(WiniStatus.Cancelled)]
    [InlineData(WiniStatus.Sent)]
    [InlineData(WiniStatus.SendError)]
    [InlineData(WiniStatus.ToBeAuthorized)]
    [InlineData(WiniStatus.Saved)]
    [InlineData(WiniStatus.NotAuthorizedOnTime)]
    public void Fail_To_Set_Wini_Status_SendError(WiniStatus status)
    {
        var bookingStatus = new BookingStatus(status, DateTime.UtcNow);

        var ex = Assert.Throws<DomainLogicException>(() => bookingStatus.TryChangeStatus(WiniStatus.SendError));

        Assert.Equal("Status cannot be anything other than ToBeSent", ex.Message);
        Assert.Equal(WiniStatus.SendError.ToString(), ex.AttemptedValue);
        Assert.Equal("Status", ex.PropertyName);
        Assert.Empty(bookingStatus.StatusHistory);
    }

    [Fact]
    public void Set_Wini_Status_Sent()
    {
        var status = new BookingStatus(WiniStatus.ToBeSent, DateTime.UtcNow);

        status.TryChangeStatus(WiniStatus.Sent);

        Assert.Equal(WiniStatus.Sent, status.Status);
        Assert.Equal(DateTime.UtcNow.Date, status.Updated.Date);
        Assert.Single(status.StatusHistory);
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

        var ex = Assert.Throws<DomainLogicException>(() => bookingStatus.TryChangeStatus(WiniStatus.Sent));

        Assert.Equal("Status cannot be anything other than ToBeSent", ex.Message);
        Assert.Equal(WiniStatus.Sent.ToString(), ex.AttemptedValue);
        Assert.Equal("Status", ex.PropertyName);
        Assert.Empty(bookingStatus.StatusHistory);
    }

    [Fact]
    public void Status_History_Is_Appended()
    {
        var status = new BookingStatus(WiniStatus.Saved, DateTime.UtcNow);

        status.TryChangeStatus(WiniStatus.ToBeAuthorized);
        status.TryChangeStatus(WiniStatus.ToBeSent);
        status.TryChangeStatus(WiniStatus.Sent);

        Assert.Equal(3, status.StatusHistory.Count);
        Assert.Equal(WiniStatus.Saved, status.StatusHistory[0].Status);
        Assert.Equal(WiniStatus.ToBeAuthorized, status.StatusHistory[1].Status);
        Assert.Equal(WiniStatus.ToBeSent, status.StatusHistory[2].Status);
    }
}