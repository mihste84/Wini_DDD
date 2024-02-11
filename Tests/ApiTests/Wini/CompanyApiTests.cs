namespace Tests.ApiTests.Wini;

[Order(2)]
public sealed class CompanyApiTests : IClassFixture<TestBase>
{
    private readonly TestBase _testBase;

    public CompanyApiTests(TestBase testBase)
    {
        _testBase = testBase;
    }

    [Fact]
    public async Task Get_All_Companies()
    {
        await _testBase.ResetDbAsync();
        var res = await _testBase.HttpClient.GetAsync("/api/companies");
        res.EnsureSuccessStatusCode();
        var content = await res.Content.ReadFromJsonAsync<IEnumerable<CompanyDto>>();
        Assert.NotNull(content);
        Assert.Equal(4, content.Count());
    }
}