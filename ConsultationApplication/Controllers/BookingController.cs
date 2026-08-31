using System.Security.Claims;
using ConsultationApplication.Data;
using ConsultationApplication.DTOs;
using ConsultationApplication.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ConsultationApplication.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BookingController : ControllerBase
{
    private readonly ConsultationAppDbContext _context;

    public BookingController(ConsultationAppDbContext context)
    {
        _context = context;
    }

    [HttpPost("create")]
    [Authorize(Roles = "User")]
    public async Task<IActionResult> CreateBooking([FromBody] BookingDto dto)
    {
        var clientId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(clientId))
            return Unauthorized();

        var slot = await _context.Slots.SingleOrDefaultAsync(candidate => candidate.Id == dto.SlotId);
        if (slot is null)
            return NotFound(new { message = "Slot not found." });

        if (!slot.IsAvailable || slot.StartTime <= DateTime.UtcNow)
            return Conflict(new { message = "Slot is no longer available." });

        if (slot.ConsultantId != dto.ConsultantId)
            return BadRequest(new { message = "The selected slot does not belong to this consultant." });

        var consultantCanProvideService = await _context.ConsultantServices.AnyAsync(link =>
            link.Consultant.UserId == dto.ConsultantId &&
            link.Consultant.IsApproved &&
            link.ServiceId == dto.ServiceId);

        if (!consultantCanProvideService)
            return BadRequest(new { message = "The consultant is not approved to provide this service." });

        slot.IsAvailable = false;
        var booking = new Bookings
        {
            ClientId = clientId,
            ConsultantId = dto.ConsultantId,
            ServiceId = dto.ServiceId,
            SlotId = dto.SlotId
        };

        _context.Bookings.Add(booking);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            return Conflict(new { message = "The slot was booked by another client." });
        }

        return StatusCode(StatusCodes.Status201Created, new
        {
            message = "Booking created successfully.",
            bookingId = booking.Id
        });
    }

    [HttpGet("my-bookings")]
    [Authorize]
    public async Task<ActionResult<IEnumerable<BookingResponseDto>>> GetMyBookings()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
            return Unauthorized();

        var isAdmin = User.IsInRole("Admin");
        var query = _context.Bookings
            .AsNoTracking()
            .Include(booking => booking.Service)
            .Include(booking => booking.Slot)
            .Include(booking => booking.Consultant)
            .Include(booking => booking.Client)
            .AsQueryable();

        if (!isAdmin)
            query = query.Where(booking => booking.ClientId == userId || booking.ConsultantId == userId);

        var bookings = await query
            .OrderByDescending(booking => booking.Slot.StartTime)
            .Select(booking => new BookingResponseDto
            {
                Id = booking.Id,
                ClientName = booking.Client.FullName,
                ConsultantName = booking.Consultant.FullName,
                ServiceName = booking.Service.Name,
                SlotStart = booking.Slot.StartTime,
                SlotEnd = booking.Slot.EndTime,
                Status = booking.Status
            })
            .ToListAsync();

        return Ok(bookings);
    }
}