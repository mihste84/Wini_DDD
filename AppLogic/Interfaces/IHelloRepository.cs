namespace AppLogic.Interfaces;

public interface IHelloRepository
{
    Task<SqlResult?> InsertHelloTextAsync(string text, string createdBy);
    Task<IEnumerable<string>> SelectAllAsync();
    Task<bool> IsSameRowVersion(int id, byte[] rowVersion);
    Task<SqlResult> UpdateAsync(int id, string text, byte[] rowVersion, string updatedBy);
    Task<bool> DeleteByIdAsync(int id);
}