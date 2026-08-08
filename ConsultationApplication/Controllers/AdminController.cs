using ConsultationApplication.Data;
using ConsultationApplication.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ConsultationApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
   // [Authorize(Roles = "Admin")] // only admins can approve
    public class AdminController : ControllerBase
    {
        private readonly ConsultationAppDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public AdminController(ConsultationAppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // consultantId here should be string because it's the Identity UserId
        [HttpPost("approve/{consultantId}")]
        public async Task<IActionResult> ApproveConsultant(string consultantId)
        {
            var consultant = await _context.Consultants
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.UserId == consultantId);

            if (consultant == null)
                return NotFound("Consultant not found.");

            consultant.IsApproved = true;
            await _context.SaveChangesAsync();

            



            // Add Consultant role to the user
            var user = await _userManager.FindByIdAsync(consultantId);
           
            if (user == null)
                return NotFound("User not found.");

            await _userManager.RemoveFromRoleAsync(user, "User");
            await _userManager.AddToRoleAsync(user, "Consultant");

            return Ok("Consultant approved successfully.");
        }
    }
}