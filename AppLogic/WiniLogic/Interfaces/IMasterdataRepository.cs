namespace AppLogic.WiniLogic.Interfaces;

public interface IMasterdataRepository
{
    Task<IEnumerable<Company>> SelectAllCompaniesAsync();
}