namespace API.Models;

public record UpdateHelloInput
{
    public string? Input { get; set; }
    public byte[]? RowVersion { get; set; }
}