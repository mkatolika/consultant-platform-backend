using ConsultationApplication.Data;
using ConsultationApplication.DTOs;
using ConsultationApplication.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
namespace ConsultationApplication.Controllers
{

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

        // GET: api/Consultants/by-service/5
        [HttpGet("by-service/{serviceId}")]
        public async Task<IActionResult> GetConsultantsByService(int serviceId)
        {
            var consultants = await _context.ConsultantServices
                .Where(cs => cs.ServiceId == serviceId)
                .Select(cs => new ConsultantDto
                {
                    UserId = cs.Consultant.User.Id,
                    FullName = cs.Consultant.User.FullName,   // from User table
                    PhotoUrl = cs.Consultant.User.PhotoUrl,   // from User table
                    Specialization = cs.Consultant.Specialization,
                    Qualification = cs.Consultant.Qualification,
                    LicenseNumber = cs.Consultant.LicenseNumber,
                    YearsOfExperience = cs.Consultant.YearsOfExperience,
                    Rating = cs.Consultant.Rating
                })
                .ToListAsync();

            if (!consultants.Any())
                return NotFound();

            return Ok(consultants);
        }
        [HttpPost("apply")]
        public async Task<IActionResult> ApplyConsultant([FromBody] ConsultantApplicationDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var appUser = await _userManager.FindByIdAsync(userId);

            if (appUser == null)
                return BadRequest("User not found.");

            var consultant = new Consultant
            {

                UserId = userId,
                Specialization = dto.Specialization,
                Qualification = dto.Qualification,
                LicenseNumber = dto.LicenseNumber,
                YearsOfExperience = dto.YearsOfExperience,
                Rating = 0,
                IsApproved = false
            };

            _context.Consultants.Add(consultant);
            await _context.SaveChangesAsync();

            if (dto.ServiceIds != null && dto.ServiceIds.Any())
            {
                foreach (var serviceId in dto.ServiceIds)
                {
                    _context.ConsultantServices.Add(new ConsultantService
                    {
                        ConsultantId = consultant.Id,
                        ServiceId = serviceId
                    });
                }
                await _context.SaveChangesAsync();
            }

            return Ok("Consultant application submitted. Awaiting admin approval.");
        }



        [HttpGet("consultant-bookings")]
        [Authorize] // ✅ Require authentication
        public async Task<IActionResult> GetConsultantBookings()
        {
            // Extract the user ID from the JWT token
            var loggedInUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (loggedInUserId == null)
                return Unauthorized("Invalid token");

            // Optional: ensure the user is actually a consultant
            var userRoles = User.FindAll(System.Security.Claims.ClaimTypes.Role).Select(r => r.Value);
            if (!userRoles.Contains("Consultant"))
                return Forbid("You are not a consultant");

            // Get bookings assigned to this consultant
            var bookings = await _context.Bookings
                .Where(b => b.ConsultantId == loggedInUserId)
                .Include(b => b.Client)
                .Include(b => b.Consultant)
                .Include(b => b.Service)
                .Include(b => b.Slot)
                .ToListAsync();

            var dtoList = bookings.Select(b => new BookingResponseDto
            {
                Id = b.Id,
                ClientName = b.Client?.FullName,
                ConsultantName = b.Consultant?.FullName,
                ServiceName = b.Service?.Name,
                SlotStart = b.Slot?.StartTime ?? DateTime.MinValue,
                SlotEnd = b.Slot?.EndTime ?? DateTime.MinValue,
                Status = b.Status
            });

            return Ok(dtoList);
        }







        [HttpPut("{bookingId}/status")]
        public async Task<IActionResult> UpdateStatus(int bookingId, [FromBody] UpdateBookingStatusDto dto)
        {
            var booking = await _context.Bookings.FindAsync(bookingId);
            if (booking == null)
                return NotFound(new { message = "Booking not found" });

            var consultantId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (booking.ConsultantId != consultantId)
                return Forbid();

            if (booking.Status == dto.Status)
                return BadRequest(new { message = "Booking already processed" });

            booking.Status = dto.Status;


            await _context.SaveChangesAsync();

            return Ok(new { message = $"Booking {dto.Status} successfully", bookingId = booking.Id, status = booking.Status });
        }

    }
}
