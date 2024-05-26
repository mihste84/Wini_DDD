namespace Services.DatabaseDapper.Models;

public abstract record BaseModel
{
    public int? Id { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? Created { get; set; }
}