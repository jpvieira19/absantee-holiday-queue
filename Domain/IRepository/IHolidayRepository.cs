namespace Domain.IRepository;

using Domain.Model;

public interface IHolidayRepository : IGenericRepository<Holiday>
{
    Task<bool> HolidayExists(long id);
    Task<Holiday> AddHolidayPeriod(Holiday holiday, List<string> errorMessages);
    Task<Holiday> GetHolidayByIdAsync(long id);
    Task<IEnumerable<Holiday>> GetHolidaysAsync();

    Task<Holiday> AddHoliday(Holiday holiday);
    Task<Holiday> UpdateHoliday(Holiday holiday,List<string> errorMessages);
    Task<Holiday> GetHolidayByColabIdAsync(long colabId);
    

    

}
