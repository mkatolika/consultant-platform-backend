using ConsulationApplication.Data;
using ConsulationApplication.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ConsulationApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientController : ControllerBase
    {
        private readonly ConsulatationAppDBContext _context;

        public ClientController(ConsulatationAppDBContext context)
        {
            _context = context;
        }

        // GET: api/Client/my-bookings
        [HttpGet("my-bookings")]
        [Authorize]
        public async Task<IActionResult> GetMyBookings()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var bookings = await _context.Bookings
                .Where(b => b.ClientId == userId || b.ConsultantId == userId) // 👈 ID-based filter
                .Include(b => b.Service)
                .Include(b => b.Slot)
                .Include(b => b.Consultant)
                .Include(b => b.Client)
                .ToListAsync();

            var dtoList = bookings.Select(b => new BookingResponseDto
            {
                Id = b.Id,
                ClientName = b.Client?.FullName ?? "",
                ConsultantName = b.Consultant?.FullName ?? "",
                ServiceName = b.Service?.Name ?? "",
                SlotStart = b.Slot?.StartTime ?? DateTime.MinValue,
                SlotEnd = b.Slot?.EndTime ?? DateTime.MinValue,
                Status = b.Status
            });

            return Ok(dtoList);
        }
    }
}
