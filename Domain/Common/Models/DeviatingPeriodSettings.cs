namespace Domain.Common.Models;

public record DeviatingPeriodSettings
{
    public List<int> DeviatingPeriods { get; init; } = [];
    public int NormalPeriodEnd { get; init; } = 4;
    public int DeviatingPeriodEnd { get; init; } = 10;
}