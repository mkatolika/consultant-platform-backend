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
public class SlotsController : ControllerBase
{
    private readonly ConsultationAppDbContext _context;

    public SlotsController(ConsultationAppDbContext context)
    {
        _context = context;
    }

    [HttpPost("create")]
    [Authorize(Roles = "Consultant")]
    public async Task<IActionResult> CreateSlot([FromBody] CreateSlotDto dto)
    {
        var consultantId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(consultantId))
            return Unauthorized();

        var isApproved = await _context.Consultants.AnyAsync(consultant =>
            consultant.UserId == consultantId && consultant.IsApproved);
        if (!isApproved)
            return Forbid();

        var overlaps = await _context.Slots.AnyAsync(slot =>
            slot.ConsultantId == consultantId &&
            dto.StartTime < slot.EndTime &&
            dto.EndTime > slot.StartTime);
        if (overlaps)
            return Conflict(new { message = "This slot overlaps existing availability." });

        var slot = new Slot
        {
            ConsultantId = consultantId,
            StartTime = dto.StartTime.ToUniversalTime(),
            EndTime = dto.EndTime.ToUniversalTime(),
            IsAvailable = true
        };

        _context.Slots.Add(slot);
        await _context.SaveChangesAsync();

        return StatusCode(StatusCodes.Status201Created, new
        {
            message = "Slot created successfully.",
            slotId = slot.Id
        });
    }

    [HttpGet("by-consultant/{consultantId}")]
    public async Task<ActionResult<IEnumerable<Slot>>> GetSlotsByConsultant(string consultantId)
    {
        var slots = await _context.Slots
            .AsNoTracking()
            .Where(slot => slot.ConsultantId == consultantId &&
                           slot.IsAvailable &&
                           slot.StartTime > DateTime.UtcNow)
            .OrderBy(slot => slot.StartTime)
            .ToListAsync();

        return Ok(slots);
    }
}