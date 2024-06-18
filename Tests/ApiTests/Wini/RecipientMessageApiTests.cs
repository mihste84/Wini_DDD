namespace Tests.ApiTests.Wini;

[Order(5)]
public sealed class RecipientMessageApiTests : IClassFixture<BaseDbTestFixture>, IDisposable
{
    private readonly BaseDbTestFixture _testBase;
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _httpClient;

    public RecipientMessageApiTests(BaseDbTestFixture testBase)
    {
        _testBase = testBase;
        _factory = new CustomWebApplicationFactory(testBase.GetConnectionString());
        _httpClient = _factory.CreateClient();
    }

    [Fact]
    public async Task Insert_New_RecipientMessage_Async()
    {
        await _testBase.ResetDbAsync();
        var sqlResult = await _testBase.SeedBaseBookingAsync(default, default);
        var command = new RecipientMessageInput("XMIHST", "TEST");

        var res = await _httpClient.PostAsJsonAsync($"/api/booking/{sqlResult.Id}/recipient", command);
        Assert.Equal(System.Net.HttpStatusCode.Created, res.StatusCode);

        var content = await res.Content.ReadFromJsonAsync<SqlResult>();
        Assert.NotNull(content);

        var RecipientMessages = await _testBase.QueryAsync<DatabaseCommon.Models.RecipientMessage>(
            "SELECT * FROM dbo.RecipientMessages WHERE BookingId = @BookingId", new { BookingId = sqlResult.Id }
        );
        Assert.Single(RecipientMessages);
        Assert.Equal(command.Message, RecipientMessages.FirstOrDefault()?.Value);
        Assert.Equal(command.Recipient, RecipientMessages.FirstOrDefault()?.Recipient);
    }

    [Fact]
    public async Task Update_RecipientMessage_Async()
    {
        await _testBase.ResetDbAsync();
        var createdDate = DateTime.UtcNow;
        var messages = new[] {
            new DatabaseCommon.Models.RecipientMessage {
                Created = createdDate,
                CreatedBy = "MIHSTE",
                Recipient = "XMIHST",
                Value = "ASDFG"
            },
            new DatabaseCommon.Models.RecipientMessage {
                Created = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Local),
                CreatedBy = "MIHSTE",
                Recipient = "RECP2",
                Value = "XYZ"
            }
        };
        var sqlResult = await _testBase.SeedBaseBookingAsync(default, default, default, messages);

        var command = new RecipientMessageInput("XMIHST", "TEST");

        var res = await _httpClient.PatchAsJsonAsync($"/api/booking/{sqlResult.Id}/recipient", command);
        Assert.Equal(System.Net.HttpStatusCode.OK, res.StatusCode);

        var content = await res.Content.ReadFromJsonAsync<SqlResult>();
        Assert.NotNull(content);

        var dbRecipientMessages = await _testBase.QueryAsync<DatabaseCommon.Models.RecipientMessage>(
            "SELECT * FROM dbo.RecipientMessages WHERE BookingId = @BookingId ORDER BY Created DESC",
            new { BookingId = sqlResult.Id }
        );
        Assert.Equal(2, dbRecipientMessages.Count());
        Assert.Contains(dbRecipientMessages, _ => _.Recipient == "XMIHST" && _.Value == "TEST");
        Assert.Contains(dbRecipientMessages, _ => _.Recipient == "RECP2" && _.Value == "XYZ");
    }

    [Fact]
    public async Task Delete_RecipientMessage_Async()
    {
        await _testBase.ResetDbAsync();
        var createdDate = DateTime.UtcNow;
        var messages = new[] {
            new DatabaseCommon.Models.RecipientMessage {
                Created = createdDate,
                CreatedBy = "MIHSTE",
                Recipient = "XMIHST",
                Value = "ASDFG"
            },
            new DatabaseCommon.Models.RecipientMessage {
                Created = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Local),
                CreatedBy = "MIHSTE",
                Recipient = "RECP2",
                Value = "XYZ"
            }
        };
        var sqlResult = await _testBase.SeedBaseBookingAsync(default, default, default, messages);

        var res = await _httpClient.DeleteAsync(
            $"/api/booking/{sqlResult.Id}/recipient?recipient=XMIHST"
        );
        Assert.Equal(System.Net.HttpStatusCode.OK, res.StatusCode);

        var content = await res.Content.ReadFromJsonAsync<SqlResult>();
        Assert.NotNull(content);

        var dbRecipientMessages = await _testBase.QueryAsync<DatabaseCommon.Models.RecipientMessage>(
            "SELECT * FROM dbo.RecipientMessages WHERE BookingId = @BookingId ORDER BY Created DESC",
            new { BookingId = sqlResult.Id }
        );

        Assert.Single(dbRecipientMessages);
        Assert.Equal("XYZ", dbRecipientMessages.FirstOrDefault()?.Value);
        Assert.Equal("RECP2", dbRecipientMessages.FirstOrDefault()?.Recipient);
    }

    public void Dispose()
    {
        _factory.Dispose();
    }
}