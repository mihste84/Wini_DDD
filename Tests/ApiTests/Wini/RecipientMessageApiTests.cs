namespace Tests.ApiTests.Wini;

[Order(5)]
public sealed class RecipientMessageApiTests(TestBase testBase) : IClassFixture<TestBase>
{
    private readonly TestBase _testBase = testBase;

    [Fact]
    public async Task Insert_New_RecipientMessage_Async()
    {
        await _testBase.ResetDbAsync();
        var sqlResult = await _testBase.SeedBaseBookingAsync(default, default);
        var command = new RecipientMessageInput("XMIHST", "TEST");

        var res = await _testBase.HttpClient.PostAsJsonAsync($"/api/booking/{sqlResult.Id}/recipient", command);
        Assert.Equal(System.Net.HttpStatusCode.Created, res.StatusCode);

        var content = await res.Content.ReadFromJsonAsync<SqlResult>();
        Assert.NotNull(content);

        var RecipientMessages = await _testBase.QueryAsync<Services.DatabaseDapper.Models.RecipientMessage>(
            "SELECT * FROM dbo.RecipientMessages WHERE BookingId = @BookingId", new { BookingId = sqlResult.Id }
        );
        Assert.Single(RecipientMessages);
        Assert.Equal(command.Value, RecipientMessages.FirstOrDefault()?.Value);
        Assert.Equal(command.Recipient, RecipientMessages.FirstOrDefault()?.Recipient);
    }

    [Fact]
    public async Task Update_RecipientMessage_Async()
    {
        await _testBase.ResetDbAsync();
        var createdDate = DateTime.UtcNow;
        var messages = new[] {
            new Services.DatabaseDapper.Models.RecipientMessage {
                Created = createdDate,
                CreatedBy = "MIHSTE",
                Recipient = "XMIHST",
                Value = "ASDFG"
            },
            new Services.DatabaseDapper.Models.RecipientMessage {
                Created = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Local),
                CreatedBy = "MIHSTE",
                Recipient = "RECP2",
                Value = "XYZ"
            }
        };
        var sqlResult = await _testBase.SeedBaseBookingAsync(default, default, default, messages);

        var command = new RecipientMessageInput("XMIHST", "TEST");

        var res = await _testBase.HttpClient.PatchAsJsonAsync($"/api/booking/{sqlResult.Id}/recipient", command);
        Assert.Equal(System.Net.HttpStatusCode.OK, res.StatusCode);

        var content = await res.Content.ReadFromJsonAsync<SqlResult>();
        Assert.NotNull(content);

        var dbRecipientMessages = await _testBase.QueryAsync<Services.DatabaseDapper.Models.RecipientMessage>(
            "SELECT * FROM dbo.RecipientMessages WHERE BookingId = @BookingId ORDER BY Created DESC",
            new { BookingId = sqlResult.Id }
        );
        Assert.Equal(2, dbRecipientMessages.Count());
        Assert.Equal("TEST", dbRecipientMessages.FirstOrDefault()?.Value);
        Assert.Equal("XMIHST", dbRecipientMessages.FirstOrDefault()?.Recipient);
        Assert.Equal("XYZ", dbRecipientMessages.LastOrDefault()?.Value);
        Assert.Equal("RECP2", dbRecipientMessages.LastOrDefault()?.Recipient);
    }

    [Fact]
    public async Task Delete_RecipientMessage_Async()
    {
        await _testBase.ResetDbAsync();
        var createdDate = DateTime.UtcNow;
        var messages = new[] {
            new Services.DatabaseDapper.Models.RecipientMessage {
                Created = createdDate,
                CreatedBy = "MIHSTE",
                Recipient = "XMIHST",
                Value = "ASDFG"
            },
            new Services.DatabaseDapper.Models.RecipientMessage {
                Created = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Local),
                CreatedBy = "MIHSTE",
                Recipient = "RECP2",
                Value = "XYZ"
            }
        };
        var sqlResult = await _testBase.SeedBaseBookingAsync(default, default, default, messages);

        var res = await _testBase.HttpClient.DeleteAsync(
            $"/api/booking/{sqlResult.Id}/recipient?recipient=XMIHST"
        );
        Assert.Equal(System.Net.HttpStatusCode.OK, res.StatusCode);

        var content = await res.Content.ReadFromJsonAsync<SqlResult>();
        Assert.NotNull(content);

        var dbRecipientMessages = await _testBase.QueryAsync<Services.DatabaseDapper.Models.RecipientMessage>(
            "SELECT * FROM dbo.RecipientMessages WHERE BookingId = @BookingId ORDER BY Created DESC",
            new { BookingId = sqlResult.Id }
        );

        Assert.Single(dbRecipientMessages);
        Assert.Equal("XYZ", dbRecipientMessages.FirstOrDefault()?.Value);
        Assert.Equal("RECP2", dbRecipientMessages.FirstOrDefault()?.Recipient);
    }
}