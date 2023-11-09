namespace Domain.Wini.Values;

public record Money
{
    public decimal? Amount { get; }
    public Currency? Currency { get; }

    public Money(decimal? amount, Currency currency)
    {
        Amount = amount;
        Currency = currency;
    }
}