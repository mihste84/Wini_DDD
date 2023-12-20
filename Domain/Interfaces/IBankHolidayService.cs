namespace Domain.Interfaces;

public interface IBankHolidayService
{
    Task<IEnumerable<DateTime>> GetBankHolidaysAsync();
}