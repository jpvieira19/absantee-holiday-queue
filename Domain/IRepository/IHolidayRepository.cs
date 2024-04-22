namespace Domain.IRepository;

using Domain.Model;

public interface IHolidayRepository : IGenericRepository<Holiday>
{
    Task<bool> HolidayExists(long id);
    Task<Holiday> GetHolidayByIdAsync(long id);
    Task<IEnumerable<Holiday>> GetHolidaysAsync();

    Task<Holiday> AddHoliday(Holiday holiday);
    Task<Holiday> GetHolidayByColabIdAsync(long colabId);
    

    

}
