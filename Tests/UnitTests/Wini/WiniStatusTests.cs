namespace Tests.UnitTests.Wini;

public class WiniStatusTests
{
    private const string User = "MIHSTE";

    [Fact]
    public void Create_Wini_Status()
    {
        var status = new BookingStatus(WiniStatus.Saved, DateTime.UtcNow, User);
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
        var bookingStatus = new BookingStatus(status, DateTime.UtcNow, User);
        bookingStatus.TryChangeStatus(WiniStatus.Saved, User);

        Assert.Equal(WiniStatus.Saved, bookingStatus.Status);
        Assert.Equal(DateTime.UtcNow.Date, bookingStatus.Updated.Date);
        Assert.Equal(User, bookingStatus.UpdatedBy.UserId);
        Assert.Single(bookingStatus.StatusHistory);
    }

    [Theory]
    [InlineData(WiniStatus.Sent)]
    [InlineData(WiniStatus.Cancelled)]
    public void Fail_To_Set_Wini_Status_Saved(WiniStatus status)
    {
        var bookingStatus = new BookingStatus(status, DateTime.UtcNow, User);

        var ex = Assert.Throws<DomainLogicException>(() => bookingStatus.TryChangeStatus(WiniStatus.Saved, User));

        Assert.Equal("Status cannot be Sent or Cancelled", ex.Message);
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
        var bookingStatus = new BookingStatus(status, DateTime.UtcNow, User);

        bookingStatus.TryChangeStatus(WiniStatus.Cancelled, User);

        Assert.Equal(WiniStatus.Cancelled, bookingStatus.Status);
        Assert.Equal(DateTime.UtcNow.Date, bookingStatus.Updated.Date);
        Assert.Single(bookingStatus.StatusHistory);
    }

    [Theory]
    [InlineData(WiniStatus.Sent)]
    [InlineData(WiniStatus.Cancelled)]
    public void Fail_To_Set_Wini_Status_Cancelled(WiniStatus status)
    {
        var bookingStatus = new BookingStatus(status, DateTime.UtcNow, User);

        var ex = Assert.Throws<DomainLogicException>(() => bookingStatus.TryChangeStatus(WiniStatus.Cancelled, User));

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
        var bookingStatus = new BookingStatus(status, DateTime.UtcNow, User);

        bookingStatus.TryChangeStatus(WiniStatus.ToBeSent, User);

        Assert.Equal(WiniStatus.ToBeSent, bookingStatus.Status);
        Assert.Equal(DateTime.UtcNow.Date, bookingStatus.Updated.Date);
        Assert.Single(bookingStatus.StatusHistory);
    }

    [Theory]
    [InlineData(WiniStatus.ToBeSent)]
    public void Set_Wini_Status_SendError(WiniStatus status)
    {
        var bookingStatus = new BookingStatus(status, DateTime.UtcNow, User);

        bookingStatus.TryChangeStatus(WiniStatus.SendError, User);

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
        var bookingStatus = new BookingStatus(status, DateTime.UtcNow, User);

        var ex = Assert.Throws<DomainLogicException>(() => bookingStatus.TryChangeStatus(WiniStatus.ToBeSent, User));

        Assert.Equal("Status cannot be anything other than Saved or ToBeAuthorized", ex.Message);
        Assert.Equal(WiniStatus.ToBeSent.ToString(), ex.AttemptedValue);
        Assert.Equal("Status", ex.PropertyName);
        Assert.Empty(bookingStatus.StatusHistory);
    }

    [Fact]
    public void Set_Wini_Status_NotAuthorizedOnTime()
    {
        var status = new BookingStatus(WiniStatus.ToBeAuthorized, DateTime.UtcNow, User);

        status.TryChangeStatus(WiniStatus.NotAuthorizedOnTime, User);

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
        var bookingStatus = new BookingStatus(status, DateTime.UtcNow, User);

        var ex = Assert.Throws<DomainLogicException>(() => bookingStatus.TryChangeStatus(WiniStatus.NotAuthorizedOnTime, User));

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
        var bookingStatus = new BookingStatus(status, DateTime.UtcNow, User);

        var ex = Assert.Throws<DomainLogicException>(() => bookingStatus.TryChangeStatus(WiniStatus.SendError, User));

        Assert.Equal("Status cannot be anything other than ToBeSent", ex.Message);
        Assert.Equal(WiniStatus.SendError.ToString(), ex.AttemptedValue);
        Assert.Equal("Status", ex.PropertyName);
        Assert.Empty(bookingStatus.StatusHistory);
    }

    [Fact]
    public void Set_Wini_Status_Sent()
    {
        var status = new BookingStatus(WiniStatus.ToBeSent, DateTime.UtcNow, User);

        status.TryChangeStatus(WiniStatus.Sent, User);

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
        var bookingStatus = new BookingStatus(status, DateTime.UtcNow, User);

        var ex = Assert.Throws<DomainLogicException>(() => bookingStatus.TryChangeStatus(WiniStatus.Sent, User));

        Assert.Equal("Status cannot be anything other than ToBeSent", ex.Message);
        Assert.Equal(WiniStatus.Sent.ToString(), ex.AttemptedValue);
        Assert.Equal("Status", ex.PropertyName);
        Assert.Empty(bookingStatus.StatusHistory);
    }

    [Fact]
    public void Status_History_Is_Appended()
    {
        var status = new BookingStatus(WiniStatus.Saved, DateTime.UtcNow, User);

        status.TryChangeStatus(WiniStatus.ToBeAuthorized, User);
        status.TryChangeStatus(WiniStatus.ToBeSent, User);
        status.TryChangeStatus(WiniStatus.Sent, User);

        Assert.Equal(3, status.StatusHistory.Count);
        Assert.Equal(WiniStatus.Saved, status.StatusHistory[0].Status);
        Assert.Equal(User, status.StatusHistory[0].UpdatedBy.UserId);
        Assert.Equal(WiniStatus.ToBeAuthorized, status.StatusHistory[1].Status);
        Assert.Equal(User, status.StatusHistory[1].UpdatedBy.UserId);
        Assert.Equal(WiniStatus.ToBeSent, status.StatusHistory[2].Status);
        Assert.Equal(User, status.StatusHistory[2].UpdatedBy.UserId);
    }
}