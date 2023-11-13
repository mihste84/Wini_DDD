namespace Domain.Wini.Values;

public record Currency
{
    public CurrencyCode CurrencyCode { get; }
    public decimal? CurrencyRate { get; }
    public Currency()
    {
        CurrencyCode = new CurrencyCode();
    }
    public Currency(CurrencyCode code, decimal? currencyRate = 0)
    {
        CurrencyCode = code;
        CurrencyRate = currencyRate;
    }
}