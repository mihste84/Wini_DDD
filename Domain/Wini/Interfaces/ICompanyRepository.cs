namespace Domain.Wini.Interfaces;

public interface ICompanyRepository
{
    Task<IEnumerable<Company>> SelectAllCompaniesAsync();
}