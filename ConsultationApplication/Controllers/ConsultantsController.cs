using System.Security.Claims;
using ConsultationApplication.Data;
using ConsultationApplication.DTOs;
using ConsultationApplication.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ConsultationApplication.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ConsultantsController : ControllerBase
{
    private readonly ConsultationAppDbContext _context;
    private readonly UserManager<AppUser> _userManager;

    public ConsultantsController(ConsultationAppDbContext context, UserManager<AppUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [HttpGet("by-service/{serviceId:int}")]
    public async Task<ActionResult<IEnumerable<ConsultantDto>>> GetConsultantsByService(int serviceId)
    {
        var consultants = await _context.ConsultantServices
            .AsNoTracking()
            .Where(link => link.ServiceId == serviceId && link.Consultant.IsApproved)
            .OrderByDescending(link => link.Consultant.Rating)
            .Select(link => new ConsultantDto
            {
                UserId = link.Consultant.User.Id,
                FullName = link.Consultant.User.FullName,
                PhotoUrl = link.Consultant.User.PhotoUrl,
                Specialization = link.Consultant.Specialization,
                Qualification = link.Consultant.Qualification,
                LicenseNumber = link.Consultant.LicenseNumber,
                YearsOfExperience = link.Consultant.YearsOfExperience,
                Rating = link.Consultant.Rating
            })
            .ToListAsync();

        return Ok(consultants);
    }

    [HttpPost("apply")]
    [Authorize(Roles = "User")]
    public async Task<IActionResult> ApplyConsultant([FromBody] ConsultantApplicationDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId) || await _userManager.FindByIdAsync(userId) is null)
            return Unauthorized();

        if (await _context.Consultants.AnyAsync(consultant => consultant.UserId == userId))
            return Conflict(new { message = "You have already submitted a consultant application." });

        var licenseNumber = dto.LicenseNumber.Trim();
        if (await _context.Consultants.AnyAsync(consultant => consultant.LicenseNumber == licenseNumber))
            return Conflict(new { message = "This licence number is already registered." });

        var serviceIds = dto.ServiceIds.Distinct().ToList();
        var existingServiceCount = await _context.Services.CountAsync(service => serviceIds.Contains(service.Id));
        if (existingServiceCount != serviceIds.Count)
            return BadRequest(new { message = "One or more selected services do not exist." });

        var consultant = new Consultant
        {
            UserId = userId,
            Specialization = dto.Specialization.Trim(),
            Qualification = dto.Qualification.Trim(),
            LicenseNumber = licenseNumber,
            YearsOfExperience = dto.YearsOfExperience
        };

        foreach (var serviceId in serviceIds)
            consultant.ConsultantServices.Add(new ConsultantService { ServiceId = serviceId });

        _context.Consultants.Add(consultant);
        await _context.SaveChangesAsync();

        return StatusCode(StatusCodes.Status201Created, new
        {
            message = "Consultant application submitted and awaiting admin approval.",
            consultantId = consultant.Id
        });
    }

    [HttpGet("consultant-bookings")]
    [Authorize(Roles = "Consultant")]
    public async Task<ActionResult<IEnumerable<BookingResponseDto>>> GetConsultantBookings()
    {
        var consultantId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(consultantId))
            return Unauthorized();

        var bookings = await _context.Bookings
            .AsNoTracking()
            .Where(booking => booking.ConsultantId == consultantId)
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

    [HttpPut("{bookingId:int}/status")]
    [Authorize(Roles = "Consultant")]
    public async Task<IActionResult> UpdateStatus(int bookingId, [FromBody] UpdateBookingStatusDto dto)
    {
        if (dto.Status is not (Bookings.BookingStatus.Accepted or
            Bookings.BookingStatus.Rejected or
            Bookings.BookingStatus.Completed))
        {
            return BadRequest(new { message = "Consultants can only accept, reject, or complete bookings." });
        }

        var consultantId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var booking = await _context.Bookings.FindAsync(bookingId);
        if (booking is null)
            return NotFound(new { message = "Booking not found." });

        if (booking.ConsultantId != consultantId)
            return Forbid();

        if (booking.Status == dto.Status)
            return Conflict(new { message = "Booking already has this status." });

        if (booking.Status is Bookings.BookingStatus.Cancelled or Bookings.BookingStatus.Rejected)
            return Conflict(new { message = "A closed booking cannot be updated." });

        booking.Status = dto.Status;
        await _context.SaveChangesAsync();

        return Ok(new { message = $"Booking marked as {dto.Status}.", booking.Id, booking.Status });
    }
}
