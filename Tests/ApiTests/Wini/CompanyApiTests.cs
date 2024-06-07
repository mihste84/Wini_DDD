namespace Tests.ApiTests.Wini;

[Order(4)]
public sealed class CompanyApiTests : IClassFixture<BaseDbTestFixture>, IDisposable
{
    private readonly BaseDbTestFixture _testBase;
    private readonly DatabaseWebApplicationFactory _factory;
    private readonly HttpClient _httpClient;

    public CompanyApiTests(BaseDbTestFixture testBase)
    {
        _testBase = testBase;
        _factory = new DatabaseWebApplicationFactory(testBase.GetConnectionString());
        _httpClient = _factory.CreateClient();
    }

    public void Dispose()
    {
        _factory.Dispose();
    }

    [Fact]
    public async Task Get_All_Companies_Async()
    {
        await _testBase.ResetDbAsync();
        var res = await _httpClient.GetAsync("api/masterdata");
        res.EnsureSuccessStatusCode();
        var content = await res.Content.ReadFromJsonAsync<IEnumerable<CompanyDto>>();
        Assert.NotNull(content);
        Assert.Equal(4, content.Count());
    }
}