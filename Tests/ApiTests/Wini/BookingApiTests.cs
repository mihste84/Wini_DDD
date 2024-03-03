namespace Tests.ApiTests.Wini;

[Order(3)]
public sealed class BookingApiTests(TestBase testBase) : IClassFixture<TestBase>
{
    private readonly TestBase _testBase = testBase;

    [Fact]
    public async Task Insert_New_Empty_Booking_Async()
    {
        await _testBase.ResetDbAsync();
        var command = new InsertNewBookingCommand
        {
            Rows = [
                new BookingRowModel() { RowNumber = 1 },
                new BookingRowModel() { RowNumber = 2 }
            ]
        };

        var res = await _testBase.HttpClient.PostAsJsonAsync("/api/booking", command);
        var content = await res.Content.ReadFromJsonAsync<SqlResult>();
        var booking = await _testBase.QuerySingleAsync<Services.DatabaseDapper.Models.Booking>("SELECT TOP 1 * FROM dbo.Bookings");
        var numberOfRows = await _testBase.QuerySingleAsync<int>("SELECT count(*) FROM dbo.BookingRows");
        Assert.Equal(System.Net.HttpStatusCode.Created, res.StatusCode);
        Assert.NotNull(content);
        Assert.True(content.Id > 0);
        Assert.Equal(2, numberOfRows);
        Assert.Equal(content.Id, booking!.Id);
        Assert.Equal(content.RowVersion, booking.RowVersion);
        Assert.Equal(WiniStatus.Saved, (WiniStatus)booking.Status.GetValueOrDefault());
        Assert.Equal(Ledgers.AA, (Ledgers)booking.LedgerType.GetValueOrDefault());
        Assert.Equal("", booking.TextToE1);
        Assert.False(booking.Reversed);
        Assert.Equal(DateTime.UtcNow.Date, booking.BookingDate);
    }

    [Fact]
    public async Task Insert_New_Booking_Async()
    {
        await _testBase.ResetDbAsync();
        var rowToInsert = new BookingRowModel
        {
            RowNumber = 1,
            Account = "12345",
            Amount = 100,
            Authorizer = "MIHSTE@mail.com",
            BusinessUnit = "100KKTOT",
            CostObject1 = "CO1",
            CostObject2 = "CO2",
            CostObject3 = "CO3",
            CostObject4 = "CO4",
            CostObjectType1 = "1",
            CostObjectType2 = "2",
            CostObjectType3 = "3",
            CostObjectType4 = "4",
            CurrencyCode = "NOK",
            ExchangeRate = 1.2m,
            Subledger = "XYZ",
            SubledgerType = "A",
            Remark = "TEST BOOKING ROW",
            Subsidiary = "1234"
        };
        var command = new InsertNewBookingCommand
        {
            BookingDate = new DateTime(2024, 1, 1),
            IsReversed = true,
            LedgerType = Ledgers.GP,
            TextToE1 = "Test",
            Rows = [rowToInsert]
        };

        var res = await _testBase.HttpClient.PostAsJsonAsync("/api/booking", command);
        Assert.Equal(System.Net.HttpStatusCode.Created, res.StatusCode);

        var content = await res.Content.ReadFromJsonAsync<SqlResult>();
        Assert.NotNull(content);

        var booking = await _testBase.QuerySingleAsync<Services.DatabaseDapper.Models.Booking>("SELECT TOP 1 * FROM dbo.Bookings");
        var row = await _testBase.QuerySingleAsync<Services.DatabaseDapper.Models.BookingRow>("SELECT TOP 1 * FROM dbo.BookingRows");
        Assert.NotNull(booking);
        Assert.NotNull(row);
        Assert.True(content.Id > 0);
        Assert.Equal(content.Id, booking.Id);
        Assert.Equal(content.RowVersion, booking.RowVersion);
        Assert.Equal(command.BookingDate, booking.BookingDate);
        Assert.Equal(command.TextToE1, booking.TextToE1);
        Assert.Equal(command.IsReversed, booking.Reversed);
        Assert.Equal(WiniStatus.Saved, (WiniStatus)booking.Status.GetValueOrDefault());
        Assert.Equal(Ledgers.GP, (Ledgers)booking.LedgerType.GetValueOrDefault());
        Assert.Equal(rowToInsert.Account, row.Account);
        Assert.Equal(rowToInsert.Amount, row.Amount);
        Assert.Equal(rowToInsert.Authorizer, row.Authorizer);
        Assert.Equal(rowToInsert.BusinessUnit, row.BusinessUnit);
        Assert.Equal(rowToInsert.CostObject1, row.CostObject1);
        Assert.Equal(rowToInsert.CostObject2, row.CostObject2);
        Assert.Equal(rowToInsert.CostObject3, row.CostObject3);
        Assert.Equal(rowToInsert.CostObject4, row.CostObject4);
        Assert.Equal(rowToInsert.CostObjectType1, row.CostObjectType1);
        Assert.Equal(rowToInsert.CostObjectType2, row.CostObjectType2);
        Assert.Equal(rowToInsert.CostObjectType3, row.CostObjectType3);
        Assert.Equal(rowToInsert.CostObjectType4, row.CostObjectType4);
        Assert.Equal(rowToInsert.CurrencyCode, row.CurrencyCode);
        Assert.Equal(rowToInsert.ExchangeRate, row.ExchangeRate);
        Assert.Equal(rowToInsert.Remark, row.Remark);
        Assert.Equal(rowToInsert.RowNumber, row.RowNumber);
        Assert.Equal(rowToInsert.Subledger, row.Subledger);
        Assert.Equal(rowToInsert.SubledgerType, row.SubledgerType);
        Assert.Equal(rowToInsert.Subsidiary, row.Subsidiary);
    }

    [Fact]
    public async Task Get_Booking_By_Id_Async()
    {
        await _testBase.ResetDbAsync();

        var sqlResult = await _testBase.SeedBaseBookingAsync(default, default);

        var booking = await _testBase.HttpClient.GetFromJsonAsync<BookingDto>("/api/booking/" + sqlResult!.Id);
        Assert.NotNull(booking);
    }

    [Fact]
    public async Task Update_Booking_Async()
    {
        await _testBase.ResetDbAsync();
        var sqlResult = await _testBase.SeedBaseBookingAsync(default, default);
        var rowToUpdate = new BookingRowModel
        {
            RowNumber = 1,
            Account = "11111",
            Amount = 100,
            Authorizer = "MIHSTE@mail.com",
            BusinessUnit = "100NN123",
            CostObject1 = "CO5",
            CostObjectType1 = "5",
            CurrencyCode = "SEK",
            ExchangeRate = 0,
            Remark = "TEST BOOKING ROW 1",
        };
        var rowToInsert = new BookingRowModel
        {
            RowNumber = 2,
            Account = "98765",
            Amount = -100,
            Authorizer = "",
            BusinessUnit = "100KKTOT",
            CurrencyCode = "SEK",
            ExchangeRate = 0,
            Remark = "TEST BOOKING ROW 2",
        };
        var command = new UpdateBookingCommand
        {
            BookingDate = new DateTime(2024, 3, 2),
            IsReversed = false,
            LedgerType = Ledgers.AA,
            TextToE1 = "Test edit",
            Rows = [
                rowToUpdate,
                rowToInsert
            ],
            BookingId = sqlResult.Id,
            RowVersion = sqlResult.RowVersion
        };
        var res = await _testBase.HttpClient.PatchAsJsonAsync("/api/booking/" + sqlResult.Id, command);
        Assert.Equal(System.Net.HttpStatusCode.OK, res.StatusCode);

        var content = await res.Content.ReadFromJsonAsync<SqlResult>();
        Assert.NotNull(content);

        var booking = await _testBase.QuerySingleAsync<Services.DatabaseDapper.Models.Booking>("SELECT TOP 1 * FROM dbo.Bookings");
        var rows = (await _testBase.QueryAsync<Services.DatabaseDapper.Models.BookingRow>("SELECT * FROM dbo.BookingRows ORDER BY RowNumber")).ToArray();
        Assert.NotNull(booking);
        Assert.NotEmpty(rows);
        Assert.True(content.Id > 0);
        Assert.Equal(content.Id, booking.Id);
        Assert.Equal(content.RowVersion, booking.RowVersion);
        Assert.Equal(command.BookingDate, booking.BookingDate);
        Assert.Equal(command.TextToE1, booking.TextToE1);
        Assert.Equal(command.IsReversed, booking.Reversed);
        Assert.Equal(WiniStatus.Saved, (WiniStatus)booking.Status.GetValueOrDefault());
        Assert.Equal(command.LedgerType, (Ledgers)booking.LedgerType.GetValueOrDefault());
        Assert.Equal(rowToUpdate.Account, rows[0].Account);
        Assert.Equal(rowToUpdate.Amount, rows[0].Amount);
        Assert.Equal(rowToUpdate.Authorizer, rows[0].Authorizer);
        Assert.Equal(rowToUpdate.BusinessUnit, rows[0].BusinessUnit);
        Assert.Equal(rowToUpdate.CostObject1, rows[0].CostObject1);
        Assert.Equal(rowToUpdate.CostObject2, rows[0].CostObject2);
        Assert.Equal(rowToUpdate.CostObject3, rows[0].CostObject3);
        Assert.Equal(rowToUpdate.CostObject4, rows[0].CostObject4);
        Assert.Equal(rowToUpdate.CostObjectType1, rows[0].CostObjectType1);
        Assert.Equal(rowToUpdate.CostObjectType2, rows[0].CostObjectType2);
        Assert.Equal(rowToUpdate.CostObjectType3, rows[0].CostObjectType3);
        Assert.Equal(rowToUpdate.CostObjectType4, rows[0].CostObjectType4);
        Assert.Equal(rowToUpdate.CurrencyCode, rows[0].CurrencyCode);
        Assert.Equal(rowToUpdate.ExchangeRate, rows[0].ExchangeRate);
        Assert.Equal(rowToUpdate.Remark, rows[0].Remark);
        Assert.Equal(rowToUpdate.RowNumber, rows[0].RowNumber);
        Assert.Equal(rowToUpdate.Subledger, rows[0].Subledger);
        Assert.Equal(rowToUpdate.SubledgerType, rows[0].SubledgerType);
        Assert.Equal(rowToUpdate.Subsidiary, rows[0].Subsidiary);
        Assert.Equal(rowToInsert.Account, rows[1].Account);
        Assert.Equal(rowToInsert.Amount, rows[1].Amount);
        Assert.Equal(rowToInsert.Authorizer, rows[1].Authorizer);
        Assert.Equal(rowToInsert.BusinessUnit, rows[1].BusinessUnit);
        Assert.Equal(rowToInsert.CostObject1, rows[1].CostObject1);
        Assert.Equal(rowToInsert.CostObject2, rows[1].CostObject2);
        Assert.Equal(rowToInsert.CostObject3, rows[1].CostObject3);
        Assert.Equal(rowToInsert.CostObject4, rows[1].CostObject4);
        Assert.Equal(rowToInsert.CostObjectType1, rows[1].CostObjectType1);
        Assert.Equal(rowToInsert.CostObjectType2, rows[1].CostObjectType2);
        Assert.Equal(rowToInsert.CostObjectType3, rows[1].CostObjectType3);
        Assert.Equal(rowToInsert.CostObjectType4, rows[1].CostObjectType4);
        Assert.Equal(rowToInsert.CurrencyCode, rows[1].CurrencyCode);
        Assert.Equal(rowToInsert.ExchangeRate, rows[1].ExchangeRate);
        Assert.Equal(rowToInsert.Remark, rows[1].Remark);
        Assert.Equal(rowToInsert.RowNumber, rows[1].RowNumber);
        Assert.Equal(rowToInsert.Subledger, rows[1].Subledger);
        Assert.Equal(rowToInsert.SubledgerType, rows[1].SubledgerType);
        Assert.Equal(rowToInsert.Subsidiary, rows[1].Subsidiary);
    }

    [Fact]
    public async Task Update_Booking_With_Delete_Rows_Async()
    {
        await _testBase.ResetDbAsync();
        var insertRows = new[] {
            new Services.DatabaseDapper.Models.BookingRow { RowNumber = 1, Account = "1", IsAuthorized = false },
            new Services.DatabaseDapper.Models.BookingRow { RowNumber = 2, Account = "2", IsAuthorized = false },
            new Services.DatabaseDapper.Models.BookingRow { RowNumber = 3, Account = "3", IsAuthorized = false },
            new Services.DatabaseDapper.Models.BookingRow { RowNumber = 4, Account = "4", IsAuthorized = false }
        };
        var sqlResult = await _testBase.SeedBaseBookingAsync(default, insertRows);
        var command = new UpdateBookingCommand
        {
            Rows = [
                new BookingRowModel {
                    RowNumber = 1,
                    Account = "123"
                },
                new BookingRowModel {
                    RowNumber = 3,
                    Account = "987"
                }
            ],
            RowNumbersToDelete = [2],
            BookingId = sqlResult.Id,
            RowVersion = sqlResult.RowVersion
        };

        var res = await _testBase.HttpClient.PatchAsJsonAsync("/api/booking/" + sqlResult.Id, command);
        Assert.Equal(System.Net.HttpStatusCode.OK, res.StatusCode);

        var content = await res.Content.ReadFromJsonAsync<SqlResult>();
        Assert.NotNull(content);

        var booking = await _testBase.QuerySingleAsync<Services.DatabaseDapper.Models.Booking>("SELECT TOP 1 * FROM dbo.Bookings");
        var rows = (await _testBase.QueryAsync<Services.DatabaseDapper.Models.BookingRow>("SELECT * FROM dbo.BookingRows")).ToArray();
        Assert.NotNull(booking);
        Assert.NotEmpty(rows);
        Assert.Equal(4, rows.Length);
        Assert.Contains(rows, _ => _.RowNumber == 2 && _.IsDeleted == true);
        Assert.Contains(rows, _ => new List<int> { 1, 3, 4 }.Contains(_.RowNumber.GetValueOrDefault()) && _.IsDeleted == false);
        Assert.Contains(rows, _ => _.RowNumber == 1 && _.Account == "123");
        Assert.Contains(rows, _ => _.RowNumber == 3 && _.Account == "987");
        Assert.Contains(rows, _ => _.RowNumber == 4 && _.Account == "4");
    }

    [Fact]
    public async Task Update_Booking_Status_Cancelled_Async()
    {
        await _testBase.ResetDbAsync();
        var insertRows = GetBookingRows();
        var sqlResult = await _testBase.SeedBaseBookingAsync(default, insertRows);
        var command = new UpdateBookingStatusCommand
        {
            BookingId = sqlResult.Id,
            RowVersion = sqlResult.RowVersion,
            Status = WiniStatus.Cancelled
        };

        var res = await _testBase.HttpClient.PatchAsJsonAsync("/api/bookingstatus/" + sqlResult.Id, command);
        Assert.Equal(System.Net.HttpStatusCode.OK, res.StatusCode);

        var content = await res.Content.ReadFromJsonAsync<SqlResult>();
        Assert.NotNull(content);

        var booking = await _testBase.QuerySingleAsync<Services.DatabaseDapper.Models.Booking>("SELECT TOP 1 * FROM dbo.Bookings");
        Assert.NotNull(booking);
        var logs = (
            await _testBase.QueryAsync<Services.DatabaseDapper.Models.BookingStatusLog>(
                "SELECT * FROM dbo.BookingStatusLogs WHERE BookingId = @Id",
                new { booking.Id })
            )
            .ToArray();
        Assert.NotEmpty(logs);
        Assert.Equal(WiniStatus.Cancelled, (WiniStatus)booking.Status!);
        Assert.Contains(logs, _ => _.Status == booking.Status);
    }

    [Fact]
    public async Task Update_Booking_Status_SendError_Async()
    {
        await _testBase.ResetDbAsync();
        var insertRows = GetBookingRows("Test", true);
        var bookingToInsert = GetBooking(WiniStatus.ToBeSent);
        var sqlResult = await _testBase.SeedBaseBookingAsync(bookingToInsert, insertRows);
        var command = new UpdateBookingStatusCommand
        {
            BookingId = sqlResult.Id,
            RowVersion = sqlResult.RowVersion,
            Status = WiniStatus.SendError
        };

        var res = await _testBase.HttpClient.PatchAsJsonAsync("/api/bookingstatus/" + sqlResult.Id, command);
        Assert.Equal(System.Net.HttpStatusCode.OK, res.StatusCode);

        var content = await res.Content.ReadFromJsonAsync<SqlResult>();
        Assert.NotNull(content);

        var booking = await _testBase.QuerySingleAsync<Services.DatabaseDapper.Models.Booking>("SELECT TOP 1 * FROM dbo.Bookings");
        Assert.NotNull(booking);
        var logs = (
                await _testBase.QueryAsync<Services.DatabaseDapper.Models.BookingStatusLog>("SELECT * FROM dbo.BookingStatusLogs WHERE BookingId = @Id", new { booking.Id })
            )
            .ToArray();
        var rows = (await _testBase.QueryAsync<Services.DatabaseDapper.Models.BookingRow>("SELECT * FROM dbo.BookingRows WHERE BookingId = @Id", new { booking.Id })).ToArray();
        Assert.NotEmpty(logs);
        Assert.NotEmpty(rows);
        Assert.Equal(WiniStatus.SendError, (WiniStatus)booking.Status!);
        Assert.Contains(logs, _ => _.Status == booking.Status);
        Assert.All(rows, _ => Assert.False(_.IsAuthorized));
    }

    [Fact]
    public async Task Update_Booking_Status_Saved_Async()
    {
        await _testBase.ResetDbAsync();
        var insertRows = GetBookingRows("Test", true);
        var bookingToInsert = GetBooking(WiniStatus.ToBeSent);
        var sqlResult = await _testBase.SeedBaseBookingAsync(bookingToInsert, insertRows);
        var command = new UpdateBookingStatusCommand
        {
            BookingId = sqlResult.Id,
            RowVersion = sqlResult.RowVersion,
            Status = WiniStatus.Saved
        };

        var res = await _testBase.HttpClient.PatchAsJsonAsync("/api/bookingstatus/" + sqlResult.Id, command);
        Assert.Equal(System.Net.HttpStatusCode.OK, res.StatusCode);

        var content = await res.Content.ReadFromJsonAsync<SqlResult>();
        Assert.NotNull(content);

        var booking = await _testBase.QuerySingleAsync<Services.DatabaseDapper.Models.Booking>("SELECT TOP 1 * FROM dbo.Bookings");
        Assert.NotNull(booking);
        var logs = (
                await _testBase.QueryAsync<Services.DatabaseDapper.Models.BookingStatusLog>("SELECT * FROM dbo.BookingStatusLogs WHERE BookingId = @Id", new { booking.Id })
            )
            .ToArray();
        var rows = (await _testBase.QueryAsync<Services.DatabaseDapper.Models.BookingRow>("SELECT * FROM dbo.BookingRows WHERE BookingId = @Id", new { booking.Id })).ToArray();
        Assert.NotEmpty(logs);
        Assert.NotEmpty(rows);
        Assert.Equal(WiniStatus.Saved, (WiniStatus)booking.Status!);
        Assert.Contains(logs, _ => _.Status == booking.Status);
        Assert.All(rows, _ => Assert.False(_.IsAuthorized));
    }

    [Fact]
    public async Task Update_Booking_Status_ToBeSent_Async()
    {
        await _testBase.ResetDbAsync();
        var insertRows = GetBookingRows(TestAuthenticationService.UserId, false);
        var bookingToInsert = GetBooking(WiniStatus.ToBeAuthorized, "COMM");
        var sqlResult = await _testBase.SeedBaseBookingAsync(bookingToInsert, insertRows);
        var command = new UpdateBookingStatusCommand
        {
            BookingId = sqlResult.Id,
            RowVersion = sqlResult.RowVersion,
            Status = WiniStatus.ToBeSent
        };

        var res = await _testBase.HttpClient.PatchAsJsonAsync("/api/bookingstatus/" + sqlResult.Id, command);
        Assert.Equal(System.Net.HttpStatusCode.OK, res.StatusCode);

        var content = await res.Content.ReadFromJsonAsync<SqlResult>();
        Assert.NotNull(content);

        var booking = await _testBase.QuerySingleAsync<Services.DatabaseDapper.Models.Booking>("SELECT TOP 1 * FROM dbo.Bookings");
        Assert.NotNull(booking);
        var logs = (
                await _testBase.QueryAsync<Services.DatabaseDapper.Models.BookingStatusLog>("SELECT * FROM dbo.BookingStatusLogs WHERE BookingId = @Id", new { booking.Id })
            )
            .ToArray();
        var rows = (await _testBase.QueryAsync<Services.DatabaseDapper.Models.BookingRow>("SELECT * FROM dbo.BookingRows WHERE BookingId = @Id", new { booking.Id })).ToArray();
        Assert.NotEmpty(logs);
        Assert.NotEmpty(rows);
        Assert.Equal(WiniStatus.ToBeSent, (WiniStatus)booking.Status!);
        Assert.Contains(logs, _ => _.Status == booking.Status);
        Assert.Contains(rows, _ => _.IsAuthorized == true && _.RowNumber == 1);
        Assert.Contains(rows, _ => _.IsAuthorized == false && _.RowNumber == 2);
    }

    [Fact]
    public async Task Update_Booking_Status_ToBeAuthorized_Async()
    {
        await _testBase.ResetDbAsync();
        var insertRows = GetBookingRows();
        var bookingToInsert = GetBooking(WiniStatus.Saved);
        var sqlResult = await _testBase.SeedBaseBookingAsync(bookingToInsert, insertRows);
        var command = new UpdateBookingStatusCommand
        {
            BookingId = sqlResult.Id,
            RowVersion = sqlResult.RowVersion,
            Status = WiniStatus.ToBeAuthorized
        };

        var res = await _testBase.HttpClient.PatchAsJsonAsync("/api/bookingstatus/" + sqlResult.Id, command);
        Assert.Equal(System.Net.HttpStatusCode.OK, res.StatusCode);

        var content = await res.Content.ReadFromJsonAsync<SqlResult>();
        Assert.NotNull(content);

        var booking = await _testBase.QuerySingleAsync<Services.DatabaseDapper.Models.Booking>("SELECT TOP 1 * FROM dbo.Bookings");
        Assert.NotNull(booking);
        var logs = (
            await _testBase.QueryAsync<Services.DatabaseDapper.Models.BookingStatusLog>("SELECT * FROM dbo.BookingStatusLogs WHERE BookingId = @Id", new { booking.Id })
            )
            .ToArray();
        var rows = (await _testBase.QueryAsync<Services.DatabaseDapper.Models.BookingRow>("SELECT * FROM dbo.BookingRows WHERE BookingId = @Id", new { booking.Id })).ToArray();
        Assert.NotEmpty(logs);
        Assert.NotEmpty(rows);
        Assert.Equal(WiniStatus.ToBeAuthorized, (WiniStatus)booking.Status!);
        Assert.Contains(logs, _ => _.Status == booking.Status);
        Assert.All(rows, _ => Assert.False(_.IsAuthorized));
    }

    [Fact]
    public async Task Update_Booking_Status_NotAuthorizedOnTime_Async()
    {
        await _testBase.ResetDbAsync();
        var insertRows = GetBookingRows();
        var bookingToInsert = GetBooking(WiniStatus.ToBeAuthorized);
        var sqlResult = await _testBase.SeedBaseBookingAsync(bookingToInsert, insertRows);
        var threeDaysAgo = DateTime.UtcNow.AddDays(-3);
        var rowVersion = await _testBase.QuerySingleAsync<byte[]>(
            "UPDATE dbo.Bookings SET Updated = @Updated WHERE Id = @Id; SELECT RowVersion FROM dbo.Bookings",
            new { Updated = threeDaysAgo, sqlResult.Id }
        );
        var command = new UpdateBookingStatusCommand
        {
            BookingId = sqlResult.Id,
            RowVersion = rowVersion,
            Status = WiniStatus.NotAuthorizedOnTime
        };

        var res = await _testBase.HttpClient.PatchAsJsonAsync("/api/bookingstatus/" + sqlResult.Id, command);
        Assert.Equal(System.Net.HttpStatusCode.OK, res.StatusCode);

        var content = await res.Content.ReadFromJsonAsync<SqlResult>();
        Assert.NotNull(content);

        var booking = await _testBase.QuerySingleAsync<Services.DatabaseDapper.Models.Booking>("SELECT TOP 1 * FROM dbo.Bookings");
        Assert.NotNull(booking);
        var logs = (
                await _testBase.QueryAsync<Services.DatabaseDapper.Models.BookingStatusLog>("SELECT * FROM dbo.BookingStatusLogs WHERE BookingId = @Id", new { booking.Id })
            )
            .ToArray();
        var rows = (await _testBase.QueryAsync<Services.DatabaseDapper.Models.BookingRow>("SELECT * FROM dbo.BookingRows WHERE BookingId = @Id", new { booking.Id })).ToArray();
        Assert.NotEmpty(logs);
        Assert.NotEmpty(rows);
        Assert.Equal(WiniStatus.Saved, (WiniStatus)booking.Status!);
        Assert.Contains(logs, _ => _.Status == booking.Status);
        Assert.Contains(logs, _ => _.Status == (short)WiniStatus.NotAuthorizedOnTime);
        Assert.All(rows, _ => Assert.False(_.IsAuthorized));
    }

    [Fact]
    public async Task Update_Booking_Status_Sent_Async()
    {
        await _testBase.ResetDbAsync();
        var insertRows = GetBookingRows("Test", true);
        var bookingToInsert = GetBooking(WiniStatus.ToBeSent);
        var sqlResult = await _testBase.SeedBaseBookingAsync(bookingToInsert, insertRows);
        var command = new UpdateBookingStatusCommand
        {
            BookingId = sqlResult.Id,
            RowVersion = sqlResult.RowVersion,
            Status = WiniStatus.Sent
        };

        var res = await _testBase.HttpClient.PatchAsJsonAsync("/api/bookingstatus/" + sqlResult.Id, command);
        Assert.Equal(System.Net.HttpStatusCode.OK, res.StatusCode);

        var content = await res.Content.ReadFromJsonAsync<SqlResult>();
        Assert.NotNull(content);

        var booking = await _testBase.QuerySingleAsync<Services.DatabaseDapper.Models.Booking>("SELECT TOP 1 * FROM dbo.Bookings");
        Assert.NotNull(booking);
        var logs = (
                await _testBase.QueryAsync<Services.DatabaseDapper.Models.BookingStatusLog>("SELECT * FROM dbo.BookingStatusLogs WHERE BookingId = @Id", new { booking.Id })
            )
            .ToArray();
        Assert.NotEmpty(logs);
        Assert.Equal(WiniStatus.Sent, (WiniStatus)booking.Status!);
        Assert.Contains(logs, _ => _.Status == booking.Status);
    }

    [Fact]
    public async Task Delete_Booking_Async()
    {
        await _testBase.ResetDbAsync();
        var sqlResult = await _testBase.SeedBaseBookingAsync(default, default);

        var res = await _testBase.HttpClient.DeleteAsync("/api/booking/" + sqlResult.Id);
        Assert.Equal(System.Net.HttpStatusCode.NoContent, res.StatusCode);

        var booking = await _testBase.QuerySingleAsync<Services.DatabaseDapper.Models.Booking>("SELECT TOP 1 * FROM dbo.Bookings");
        Assert.Null(booking);
        var logs = await _testBase.QueryAsync<Services.DatabaseDapper.Models.BookingStatusLog>("SELECT * FROM dbo.BookingStatusLogs WHERE BookingId = @Id", new { sqlResult.Id });
        var rows = await _testBase.QueryAsync<Services.DatabaseDapper.Models.BookingRow>("SELECT * FROM dbo.BookingRows WHERE BookingId = @Id", new { sqlResult.Id });
        Assert.Empty(logs);
        Assert.Empty(rows);
    }

    private static Services.DatabaseDapper.Models.Booking GetBooking(WiniStatus status, string createdBy = TestAuthenticationService.UserId)
    => new()
    {
        BookingDate = new DateTime(2024, 1, 1),
        Created = DateTime.UtcNow,
        CreatedBy = createdBy,
        LedgerType = (short)Ledgers.AA,
        Reversed = true,
        Status = (short)status,
        TextToE1 = "Test",
        Updated = DateTime.UtcNow,
        UpdatedBy = createdBy
    };

    private static Services.DatabaseDapper.Models.BookingRow[] GetBookingRows(string authorizer = "AUTH", bool isAuthorized = false)
    => [
        new Services.DatabaseDapper.Models.BookingRow() {
            Account = "12345",
            Amount = 100,
            Authorizer = authorizer,
            BusinessUnit = "100KKTOT",
            CostObject1 = "123",
            CostObjectType1 = "X",
            CurrencyCode = "SEK",
            ExchangeRate = 0,
            IsAuthorized = isAuthorized,
            RowNumber = 1,
            Remark = "TEST",
            Subledger = "54321",
            SubledgerType = "A",
            Subsidiary = "1234"
        },
        new Services.DatabaseDapper.Models.BookingRow() {
            Account = "98765",
            Amount = -100,
            BusinessUnit = "100KKTOT",
            CurrencyCode = "SEK",
            ExchangeRate = 0,
            RowNumber = 2,
            IsAuthorized = false
        }
    ];
}