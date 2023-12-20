namespace Domain.Wini.Values;

public record LedgerType
{
    public Ledgers Type { get; }

    public LedgerType(Ledgers type)
    {
        Type = type;
    }
}