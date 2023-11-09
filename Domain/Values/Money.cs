namespace Domain.Values;

public record Money
{
    public decimal? Amount { get; }
    public Currency Currency { get; }

    public Money(decimal? amount, Currency currency)
    {
        Amount = amount;
        Currency = currency;
    }

    public bool IsDebitRow() => Amount >= 0;
    public bool IsCreditRow() => Amount < 0;
}