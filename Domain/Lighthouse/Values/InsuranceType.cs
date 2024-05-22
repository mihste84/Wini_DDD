namespace Domain.Lighthouse.Values;

public record InsuranceType
{
    public InsuranceTypes Type { get; }

    public InsuranceType(InsuranceTypes type)
    {
        Type = type;
    }
}