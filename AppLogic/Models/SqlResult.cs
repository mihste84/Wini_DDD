namespace AppLogic.Models;

public record SqlResult
{
    public int? Id { get; set; }
    public byte[]? RowVersion { get; set; }
}