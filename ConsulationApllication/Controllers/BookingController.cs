using ConsulationApplication.Data;
using ConsulationApplication.DTOs;
using ConsulationApplication.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ConsulationApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly ConsulatationAppDBContext _context;

        public BookingController(ConsulatationAppDBContext context)
        {
            _context = context;
        }

        // POST: api/Booking/create
        [HttpPost("create")]
        [Authorize]
        public async Task<IActionResult> CreateBooking([FromBody] BookingDto dto)
        {
            var slot = await _context.Slots.FindAsync(dto.SlotId);
            if (slot == null || !slot.IsAvailable)
                return BadRequest("Slot is not available");

            // Mark slot as unavailable
            slot.IsAvailable = false;

            var booking = new Bookings
            {
                ClientId = dto.UserId,
                ConsultantId = dto.ConsultantId,
                ServiceId = dto.ServiceId,
                SlotId = dto.SlotId,
                Status = Bookings.BookingStatus.Pending // ✅ default status
            };

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Booking created successfully", bookingId = booking.Id });
        }

        // GET: api/Booking/my-bookings
        [HttpGet("my-bookings")]
        [Authorize]
        public async Task<IActionResult> GetMyBookings()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var role = User.FindFirstValue(ClaimTypes.Role);

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            IQueryable<Bookings> query = _context.Bookings
                .Include(b => b.Service)
                .Include(b => b.Slot)
                .Include(b => b.Consultant)
                .Include(b => b.Client);

            // 👑 Admin sees all bookings
            if (role != "Admin")
            {
                query = query.Where(b =>
                    b.ClientId == userId ||
                    b.ConsultantId == userId
                );
            }

            var bookings = await query.ToListAsync();

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