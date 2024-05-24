namespace Domain.Common.Models;

public record AppRoleSettings
{
    public string? AdminRole { get; set; }
    public string? ReadRole { get; set; }
    public string? WriteRole { get; set; }
    public string? AccountingUserRole { get; set; }
    public string? ControlActuaryRole { get; set; }
    public string? SpecificActuaryRole { get; set; }
}