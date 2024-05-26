using Domain.Lighthouse.Values;

namespace Tests.UnitTests.Lighthouse;

public class PolicyTests
{
    [Fact]
    public void Create_Policy()
    {
        var policy = new Policy();
        Assert.Equal(default, policy);

        policy = new Policy("12345", "1234", new CompanyCode(100));
        Assert.Equal("12345", policy.PolicyNumber);
        Assert.Equal("1234", policy.ClientNumber);
        Assert.Equal(100, policy.LegalUnit.Code);
    }
}