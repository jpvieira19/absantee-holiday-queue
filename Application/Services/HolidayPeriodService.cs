namespace Application.Services;

using Domain.Model;
using Application.DTO;

using Microsoft.EntityFrameworkCore;
using DataModel.Repository;
using Domain.IRepository;


public class HolidayPeriodService {

    private readonly AbsanteeContext _context;

    private readonly IHolidayPeriodRepository _holidayPeriodRepository;
    
    public HolidayPeriodService(IHolidayPeriodRepository holidayPeriodRepository) {
        _holidayPeriodRepository = holidayPeriodRepository;
    }

    public async Task<IEnumerable<HolidayPeriodDTO>> GetAll()
    {    
        IEnumerable<HolidayPeriod> holidayPeriods = await _holidayPeriodRepository.GetHolidayPeriodsAsync();

        IEnumerable<HolidayPeriodDTO> holidaysDTO = HolidayPeriodDTO.ToDTO(holidayPeriods);

        return holidaysDTO;
    }

    public async Task<HolidayPeriodDTO> GetHolidayPeriodById(long id)
    {    
        HolidayPeriod holidayPeriod = await _holidayPeriodRepository.GetHolidayPeriodByIdAsync(id);

        HolidayPeriodDTO holidayPeriodDTO = HolidayPeriodDTO.ToDTO(holidayPeriod);

        return holidayPeriodDTO;
    }

    public async Task<HolidayPeriodDTO> Add(HolidayPeriodDTO holidayPeriodDto, List<string> errorMessages)
    {
        /*bool bExists = await _holidayRepository.HolidayExists(holidayDTO.Id);
        if(bExists) {
            errorMessages.Add("Already exists");
            return null;
        }*/

        HolidayPeriod holidayPeriod = HolidayPeriodDTO.ToDomain(holidayPeriodDto);

        holidayPeriod = await _holidayPeriodRepository.Add(holidayPeriod);

        HolidayPeriodDTO holidayPeriodDTO = HolidayPeriodDTO.ToDTO(holidayPeriod);

        return holidayPeriodDTO;
    }

    
}