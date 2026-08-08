using ConsulationApplication.Data;
using ConsulationApplication.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ConsulationApplication.Controllers
{
    
    [ApiController]
    [Route("api/[controller]")]
    public class SlotsController : ControllerBase
    {
        private readonly ConsulatationAppDBContext _context;

        public SlotsController(ConsulatationAppDBContext context)
        {
            _context = context;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateSlot([FromBody] Slot slot)
        {
            // Always mark new slots as available
            slot.IsAvailable = true;

            _context.Slots.Add(slot);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Slot created successfully", slotId = slot.Id });
        }



        // GET: api/Slots/by-consultant/3
        [HttpGet("by-consultant/{consultantId}")]
        public async Task<IActionResult> GetSlotsByConsultant(string consultantId)
        {
            var slots = await _context.Slots
                .Where(s => s.ConsultantId == consultantId && s.IsAvailable)
                .ToListAsync();

            return Ok(slots);
        }
    }
}
