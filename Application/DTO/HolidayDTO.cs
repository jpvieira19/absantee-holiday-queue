namespace Application.DTO;

using Domain.Factory;
using Domain.Model;

public class HolidayDTO
{
	public long Id { get; set; }
	public long _colabId{ get; set; }

	public IEnumerable<HolidayPeriodDTO> _holidayPeriods { get; set; }

    public HolidayDTO() {
	}

	public HolidayDTO(long colabId,long id,List<HolidayPeriodDTO> holidayPeriods)
	{
		Id = id;
		_colabId = colabId;
		_holidayPeriods = holidayPeriods;
	}

	static public HolidayDTO ToDTO(Holiday holiday) {
		long idColab = holiday.GetColaborator();
		long id = holiday.Id;
		List <HolidayPeriodDTO> holidayPeriodsDTO = HolidayPeriodDTO.ToDTO(holiday.HolidayPeriod).ToList();
		HolidayDTO holidayDTO = new HolidayDTO(idColab,id,holidayPeriodsDTO);

		return holidayDTO;
	}

	static public IEnumerable<HolidayDTO> ToDTO(IEnumerable<Holiday> holidays)
	{
		List<HolidayDTO> holidaysDTO = new List<HolidayDTO>();

		foreach( Holiday holiday in holidays ) {
			HolidayDTO holidayDTO = ToDTO(holiday);

			holidaysDTO.Add(holidayDTO);
		}

		return holidaysDTO;
	}

	static public Holiday ToDomain(HolidayDTO holidayDTO) 
	{
		if (holidayDTO == null) 
		{
			throw new ArgumentException("holidayDTO must not be null");
		}

		List<HolidayPeriod> holidayPeriods = holidayDTO._holidayPeriods.Select(HolidayPeriodDTO.ToDomain).ToList();

		Holiday holiday = new Holiday(holidayDTO.Id,holidayDTO._colabId,holidayPeriods);

		
		// foreach (var periodDTO in holidayDTO.HolidayPeriods)
		// {
		//     HolidayPeriod period = HolidayPeriodDTO.ToDomain(periodDTO);
		//     holiday.AddHolidayPeriod(period);
		// }

		return holiday;
	}

	static public Holiday UpdateToDomain(HolidayPeriodDTO holidayPeriodDTO,IHolidayPeriodFactory holidayPeriodFactory,Holiday holiday) {

		
		holiday.AddHolidayPeriod(holidayPeriodFactory,holidayPeriodDTO.StartDate,holidayPeriodDTO.EndDate);
		
		return holiday;

	}
}