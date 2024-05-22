namespace Domain.Common.Values;

public record Currency
{
    public CurrencyCode CurrencyCode { get; }
    public decimal? ExchangeRate { get; }
    public Currency()
    {
        CurrencyCode = new CurrencyCode();
    }
    public Currency(CurrencyCode code, decimal? exchangeRate = 0)
    {
        CurrencyCode = code;
        ExchangeRate = exchangeRate;
    }
}