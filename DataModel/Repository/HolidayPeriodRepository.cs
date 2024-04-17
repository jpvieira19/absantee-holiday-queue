namespace DataModel.Repository;

using Microsoft.EntityFrameworkCore;

using DataModel.Model;
using DataModel.Mapper;

using Domain.Model;
using Domain.IRepository;
using Microsoft.EntityFrameworkCore.ChangeTracking;

public class HolidayPeriodRepository : GenericRepository<HolidayPeriod>, IHolidayPeriodRepository
{    
    HolidayPeriodMapper _holidayPeriodMapper;
    public HolidayPeriodRepository(AbsanteeContext context, HolidayPeriodMapper mapper) : base(context!)
    {
        _holidayPeriodMapper = mapper;
    }

    public async Task<IEnumerable<HolidayPeriod>> GetHolidayPeriodsAsync()
    {
        try {
            IEnumerable<HolidayPeriodDataModel> holidayPeriodsDataModel = await _context.Set<HolidayPeriodDataModel>()
                    .ToListAsync();

            IEnumerable<HolidayPeriod> holidayPeriods = _holidayPeriodMapper.ToDomain(holidayPeriodsDataModel);

            return holidayPeriods;
        }
        catch
        {
            throw;
        }
    }

    public async Task<HolidayPeriod> GetHolidayPeriodByIdAsync(long id)
    {
        try {
            HolidayPeriodDataModel holidayPeriodDataModel = await _context.Set<HolidayPeriodDataModel>()
                    .FirstAsync(c => c.Id==id);

            HolidayPeriod holidayPeriod = _holidayPeriodMapper.ToDomain(holidayPeriodDataModel);

            return holidayPeriod;
        }
        catch
        {
            throw;
        }
    }

    public async Task<HolidayPeriod> Add(HolidayPeriod holidayPeriod)
    {
        try {
            HolidayPeriodDataModel holidayPeriodDataModel = _holidayPeriodMapper.ToDataModel(holidayPeriod);

            EntityEntry<HolidayPeriodDataModel> holidayPeriodDataModelEntityEntry = _context.Set<HolidayPeriodDataModel>().Add(holidayPeriodDataModel);
            
            await _context.SaveChangesAsync();

            HolidayPeriodDataModel holidayDataModelSaved = holidayPeriodDataModelEntityEntry.Entity;

            HolidayPeriod holidayPeriodSaved = _holidayPeriodMapper.ToDomain(holidayDataModelSaved);

            return holidayPeriodSaved;    
        }
        catch
        {
            throw;
        }
    }

    public async Task<bool> HolidayPeriodExists(long id)
    {
        return await _context.Set<HolidayPeriodDataModel>().AnyAsync(e => e.Id == id);
    }
}