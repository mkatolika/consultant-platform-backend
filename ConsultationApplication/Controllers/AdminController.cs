using ConsultationApplication.Data;
using ConsultationApplication.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ConsultationApplication.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly ConsultationAppDbContext _context;
    private readonly UserManager<AppUser> _userManager;

    public AdminController(ConsultationAppDbContext context, UserManager<AppUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [HttpPost("approve/{consultantId}")]
    public async Task<IActionResult> ApproveConsultant(string consultantId)
    {
        var consultant = await _context.Consultants
            .FirstOrDefaultAsync(candidate => candidate.UserId == consultantId);

        if (consultant is null)
            return NotFound(new { message = "Consultant application not found." });

        if (consultant.IsApproved)
            return Conflict(new { message = "Consultant is already approved." });

        var user = await _userManager.FindByIdAsync(consultantId);
        if (user is null)
            return NotFound(new { message = "Associated user not found." });

        if (await _userManager.IsInRoleAsync(user, "User"))
        {
            var removeResult = await _userManager.RemoveFromRoleAsync(user, "User");
            if (!removeResult.Succeeded)
                return Problem("Could not update the user's existing role.");
        }

        var addResult = await _userManager.AddToRoleAsync(user, "Consultant");
        if (!addResult.Succeeded)
            return Problem("Could not assign the Consultant role.");

        consultant.IsApproved = true;
        await _context.SaveChangesAsync();

        return Ok(new { message = "Consultant approved successfully." });
    }
}