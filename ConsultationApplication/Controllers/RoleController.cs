using ConsultationApplication.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ConsultationApplication.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class RoleController : ControllerBase
{
    private readonly RoleManager<IdentityRole> _roleManager;

    public RoleController(RoleManager<IdentityRole> roleManager)
    {
        _roleManager = roleManager;
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateRole(RoleDto dto)
    {
        if (await _roleManager.RoleExistsAsync(dto.Role))
            return Conflict(new { message = "Role already exists." });

        var result = await _roleManager.CreateAsync(new IdentityRole(dto.Role));
        if (!result.Succeeded)
            return BadRequest(result.Errors);

        return StatusCode(StatusCodes.Status201Created, new { message = $"Role {dto.Role} created successfully." });
    }
}