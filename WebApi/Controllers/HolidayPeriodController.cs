using Microsoft.AspNetCore.Mvc;

using Application.Services;
using Application.DTO;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HolidayPeriodController : ControllerBase
    {   
        private readonly HolidayPeriodService _holidayPeriodService;

        List<string> _errorMessages = new List<string>();

        public HolidayPeriodController(HolidayPeriodService holidayPeriodService)
        {
            _holidayPeriodService = holidayPeriodService;
        }

        // GET: api/HolidayPeriod
        [HttpGet]
        public async Task<ActionResult<IEnumerable<HolidayPeriodDTO>>> GetHolidayPeriods()
        {
            IEnumerable<HolidayPeriodDTO> holidayPeriodsDTO = await _holidayPeriodService.GetAll();

            return Ok(holidayPeriodsDTO);
        }


        // GET: api/Period/5
        [HttpGet("{id}")]
        public async Task<ActionResult<HolidayPeriodDTO>> GetHolidayPeriodById(long id)
        {
            var holidayPeriodDTO = await _holidayPeriodService.GetHolidayPeriodById(id);

            if (holidayPeriodDTO == null)
            {
                return NotFound();
            }

            return Ok(holidayPeriodDTO);
        }


        // PUT: api/Colaborator/a@bc
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        /*[HttpPut("{id}")]
        public async Task<IActionResult> PutHoliday(HolidayDTO holidayDTO)
        {

            bool wasUpdated = await _holidayService.Update(holidayDTO, _errorMessages);

            if (!wasUpdated /* && _errorMessages.Any() *//*)
            {
                return BadRequest(_errorMessages);
            }

            return Ok();
        }*/

        // POST: api/Holiday
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<HolidayPeriodDTO>> PostHolidayPeriod(HolidayPeriodDTO holidayPeriodDTO)
        {
            HolidayPeriodDTO holidayPeriodResultDTO = await _holidayPeriodService.Add(holidayPeriodDTO, _errorMessages);

            if(holidayPeriodResultDTO != null)
                return Ok(holidayPeriodResultDTO);
            else
                return BadRequest(_errorMessages);
        }

        // // DELETE: api/Colaborator/5
        // [HttpDelete("{email}")]
        // public async Task<IActionResult> DeleteColaborator(string email)
        // {
        //     var colaborator = await _context.Colaboradores.FindAsync(email);
        //     if (colaborator == null)
        //     {
        //         return NotFound();
        //     }

        //     _context.Colaboradores.Remove(colaborator);
        //     await _context.SaveChangesAsync();

        //     return NoContent();
        // }

    }
}
