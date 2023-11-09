namespace Domain.Models;

public class DeviatingPeriodSettings
{
    public List<int> DeviatingPeriods { get; init; } = new();
    public int NormalPeriodEnd { get; init; } = 4;
    public int DeviatingPeriodEnd { get; init; } = 10;
}