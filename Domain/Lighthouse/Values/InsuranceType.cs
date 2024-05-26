namespace Domain.Lighthouse.Values;

public readonly record struct InsuranceType
{
    public Insurance Type { get; }

    public InsuranceType(Insurance type)
    {
        Type = type;
    }
}