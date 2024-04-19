namespace Application.Services;

using Domain.Model;
using Application.DTO;

using Microsoft.EntityFrameworkCore;
using DataModel.Repository;
using Domain.IRepository;
using Domain.Factory;
using DataModel.Model;
using Gateway;
using System;
using System.Collections;

public class HolidayService {

    private readonly AbsanteeContext _context;
    private readonly IHolidayRepository _holidayRepository;
    private readonly IHolidayPeriodRepository _holidayPeriodRepository;
    private readonly IColaboratorsIdRepository _colaboratorsIdRepository;
    private readonly IHolidayPeriodFactory _holidayPeriodFactory;
    private readonly HolidayAmpqGateway _holidayAmqpGateway;


    
    public HolidayService(IHolidayPeriodRepository holidayPeriodRepository,IHolidayRepository holidayRepository, IHolidayPeriodFactory holidayPeriodFactory, HolidayAmpqGateway holidayAmqpGateway,IColaboratorsIdRepository colaboratorsIdRepository) {
        _holidayRepository = holidayRepository;
        _holidayPeriodFactory = holidayPeriodFactory;
        _holidayAmqpGateway=holidayAmqpGateway;
        _colaboratorsIdRepository = colaboratorsIdRepository;
        _holidayPeriodRepository = holidayPeriodRepository;
    }

    public async Task<IEnumerable<HolidayDTO>> GetAll()
    {    
        IEnumerable<Holiday> holidays = await _holidayRepository.GetHolidaysAsync();

        IEnumerable<HolidayDTO> holidaysDTO = HolidayDTO.ToDTO(holidays);

        return holidaysDTO;
    }

    public async Task<HolidayDTO> GetHolidayById(long id)
    {    
        Holiday holiday = await _holidayRepository.GetHolidayByIdAsync(id);

        HolidayDTO holidayDTO = HolidayDTO.ToDTO(holiday);

        return holidayDTO;
    }

    public async Task<HolidayDTO> Add(HolidayDTO holidayDto, List<string> errorMessages)
    {
        bool bExists = await _holidayRepository.HolidayExists(holidayDto.Id);
        bool colabExists = await _colaboratorsIdRepository.ColaboratorExists(holidayDto._colabId);
        if(bExists) {
            errorMessages.Add("Holiday already exists");
            return null;
        }
        if(!colabExists) {
            errorMessages.Add("Colab doesn't exist");
            return null;
        }

        Holiday holiday = HolidayDTO.ToDomain(holidayDto);

        holiday = await _holidayRepository.AddHoliday(holiday);

        HolidayDTO holidayDTO = HolidayDTO.ToDTO(holiday);

        return holidayDTO;
    }

    public async Task<bool> AddHolidayPeriod(long id,HolidayPeriodDTO holidayPeriodDTO,List<string> errorMessages)
    {


        Holiday holiday = await _holidayRepository.GetHolidayByIdAsync(id);

        if(holiday!=null)
        {
            HolidayDTO.UpdateToDomain(holidayPeriodDTO,_holidayPeriodFactory,holiday);
            
            holiday = await _holidayRepository.AddHolidayPeriod(holiday, errorMessages);

            HolidayDTO holidayDTO = HolidayDTO.ToDTO(holiday);

            return true;
        }
        else
        {
            errorMessages.Add("Not found");

            return false;
        }
    }

    public async Task<IEnumerable<HolidayPeriodDTO>> GetHolidayPeriodsOnHolidayById(long colabId, DateOnly startDate, DateOnly endDate,List<string> errorMessages)
    {

        Holiday holiday = await _holidayRepository.GetHolidayByColabIdAsync(colabId);
        
        if(holiday==null) {
            errorMessages.Add("Holiday doesn't exist exists");
            return null;
        }

        List<HolidayPeriod> holidayPeriods = holiday.GetHolidayPeriodsDuring(startDate,endDate);
        return HolidayPeriodDTO.ToDTO(holidayPeriods);


    }
    //fazer em vez disto, um get do repositório dos HolidayPeriods, getHolidayPeriodsByColabId no repo?,linha 133
    //fazer o foreach todo no repo, passar tudo para o repositório da HolidayPeriod?
    public async Task<List<long>> GetColabsComFeriasSuperioresAXDias(long xDias,List<string> errorMessages)
    {
        IEnumerable<ColaboratorId> lista = await _colaboratorsIdRepository.GetColaboratorsIdAsync();

        List<long> colabsComFeriasSuperioresAXDias = new List<long>();
        foreach(ColaboratorId colabId in lista){
            Holiday holiday = await _holidayRepository.GetHolidayByColabIdAsync(colabId.colabId);

            if(holiday != null){
                List<HolidayPeriod> holidayPeriods = holiday.GetHolidayPeriods();
                foreach(HolidayPeriod hp in holidayPeriods){
                    int x  = hp.GetNumberOfDays();
                    if(x>xDias){
                        colabsComFeriasSuperioresAXDias.Add(colabId.colabId);
                        break;
                    }
                }
            }
        }
        return colabsComFeriasSuperioresAXDias;

    }

    

    /*public async Task<HolidayDTO> UpdateHoliday(HolidayDTO holidayDto, List<string> errorMessages)
    {
        // Verifica se a Holiday existe
        bool exists = await _holidayRepository.HolidayExists(holidayDto.Id);
        if (!exists)
        {
            errorMessages.Add("Holiday does not exist");
            return null;
        }

        // Verifica se o colaborador existe
        bool colabExists = await _colaboratorsIdRepository.ColaboratorExists(holidayDto._colabId);
        if (!colabExists)
        {
            errorMessages.Add("Colab doesn't exist");
            return null;
        }

        IEnumerable<HolidayPeriodDTO> holidayPeriodDTOs = holidayDto._holidayPeriods;
// Converte DTO para o domínio
        Holiday holiday = HolidayDTO.ToDomain(holidayDto);

        
        HolidayDTO.UpdateToDomain(holidayPeriodDTOs,_holidayPeriodFactory,holiday);

        // Atualiza a Holiday
        holiday = await _holidayRepository.UpdateHoliday(holiday,errorMessages);

        // Converte o domínio de volta para DTO
        HolidayDTO updatedHolidayDTO = HolidayDTO.ToDTO(holiday);

        // Aqui você pode adicionar lógica para publicar a atualização para RabbitMQ se necessário
        // string holidayAmqpDTO = HolidayGatewayDTO.Serialize(updatedHolidayDTO);
        // _holidayAmqpGateway.Publish(holidayAmqpDTO);

        return updatedHolidayDTO;
    }*/

}