namespace Tests.ApiTests.Wini;

[Order(4)]
public sealed class CompanyApiTests(TestBase testBase) : IClassFixture<TestBase>
{
    private readonly TestBase _testBase = testBase;

    [Fact]
    public async Task Get_All_Companies_Async()
    {
        await _testBase.ResetDbAsync();
        var res = await _testBase.HttpClient.GetAsync("/api/companies");
        res.EnsureSuccessStatusCode();
        var content = await res.Content.ReadFromJsonAsync<IEnumerable<CompanyDto>>();
        Assert.NotNull(content);
        Assert.Equal(4, content.Count());
    }
}