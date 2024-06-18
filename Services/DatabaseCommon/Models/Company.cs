namespace DatabaseCommon.Models;

public class Company
{
    public int Id { get; set; }

    public string Code { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string CountryCode { get; set; } = null!;

    public string CreatedBy { get; set; } = null!;

    public DateTime Created { get; set; }

    public string UpdatedBy { get; set; } = null!;

    public DateTime Updated { get; set; }
}
