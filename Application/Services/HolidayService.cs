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


public class HolidayService {

    private readonly AbsanteeContext _context;
    private readonly IHolidayRepository _holidayRepository;

    private readonly IColaboratorsIdRepository _colaboratorsIdRepository;
    private readonly IHolidayPeriodFactory _holidayPeriodFactory;
    private readonly HolidayAmpqGateway _holidayAmqpGateway;


    
    public HolidayService(IHolidayRepository holidayRepository, IHolidayPeriodFactory holidayPeriodFactory, HolidayAmpqGateway holidayAmqpGateway,IColaboratorsIdRepository colaboratorsIdRepository) {
        _holidayRepository = holidayRepository;
        _holidayPeriodFactory = holidayPeriodFactory;
        _holidayAmqpGateway=holidayAmqpGateway;
        _colaboratorsIdRepository = colaboratorsIdRepository;

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

        // string holidayAmqpDTO = HolidayGatewayDTO.Serialize(holidayDTO);	
        // _holidayAmqpGateway.Publish(holidayAmqpDTO);

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