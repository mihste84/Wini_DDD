namespace Services.DatabaseDapper.Models;

public record Company
{
    public int? Id { get; set; }
    public string? Code { get; set; }
    public string? Name { get; set; }
    public string? CountryCode { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? Created { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime? Updated { get; set; }
}
