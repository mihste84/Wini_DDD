namespace Domain.Lighthouse.Values;

public record InsuranceType
{
    public Insurance Type { get; }

    public InsuranceType(Insurance type)
    {
        Type = type;
    }
}