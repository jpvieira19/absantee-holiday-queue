using Microsoft.AspNetCore.Mvc;
using Application.Services;
using Application.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HolidayController : ControllerBase
    {   
        private readonly HolidayService _holidayService;

        public HolidayController(HolidayService holidayService)
        {
            _holidayService = holidayService;
        }

        // GET: api/Holiday
        [HttpGet]
        public async Task<ActionResult<IEnumerable<HolidayDTO>>> GetHolidays()
        {
            IEnumerable<HolidayDTO> holidaysDTO = await _holidayService.GetAll();
            return Ok(holidaysDTO);
        }

        // GET: api/Holiday/5
        [HttpGet("{id}")]
        public async Task<ActionResult<HolidayDTO>> GetHolidayById(long id)
        {
            var holidayDTO = await _holidayService.GetHolidayById(id);
            if (holidayDTO == null)
            {
                return NotFound();
            }
            return Ok(holidayDTO);
        }

    }
}
