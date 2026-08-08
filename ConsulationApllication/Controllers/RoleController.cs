using ConsulationApplication.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ConsulationApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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
                return BadRequest("Role already exists");

            var result = await _roleManager.CreateAsync(new IdentityRole(dto.Role));
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok($"Role {dto.Role} created successfully");
        }




    }
}
