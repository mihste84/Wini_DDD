namespace Domain.Values;

public record Currency
{
    public CurrencyCode Code { get; }
    public decimal? CurrencyRate { get; }

    public Currency(CurrencyCode code, decimal? currencyRate = 0)
    {
        Code = code;
        CurrencyRate = currencyRate;
    }
}