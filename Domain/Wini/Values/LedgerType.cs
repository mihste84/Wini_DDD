namespace Domain.Wini.Values;

public readonly record struct LedgerType
{
    public readonly Ledgers Type;

    public LedgerType(Ledgers type)
    {
        Type = type;
    }
}