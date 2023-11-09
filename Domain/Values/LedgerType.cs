namespace Domain.Values;

public record LedgerType
{
    public Ledgers Type { get; }

    public LedgerType(Ledgers type)
    {
        Type = type;
    }

    public static LedgerType Create(Ledgers type)
    {
        // Validate
        return new LedgerType(type);
    }
}