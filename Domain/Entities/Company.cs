namespace Domain.Entities;

public class Company(IdValue<int> id, CompanyCode companyCode, CompanyName name, Country country)
{
    public readonly IdValue<int> Id = id;
    public CompanyCode CompanyCode { get; set; } = companyCode;
    public CompanyName Name { get; set; } = name;
    public Country Country { get; set; } = country;
    public readonly CurrencyCode Currency = new(country);
}