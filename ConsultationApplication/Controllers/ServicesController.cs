using ConsultationApplication.Data;
using ConsultationApplication.DTOs;
using ConsultationApplication.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ConsultationApplication.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ServicesController : ControllerBase
{
    private readonly ConsultationAppDbContext _context;

    public ServicesController(ConsultationAppDbContext context)
    {
        _context = context;
    }

    [HttpPost("create")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateService([FromBody] CreateServiceDto dto)
    {
        if (!await _context.Departments.AnyAsync(department => department.Id == dto.DepartmentId))
            return BadRequest(new { message = "Department does not exist." });

        var normalizedName = dto.Name.Trim();
        if (await _context.Services.AnyAsync(service => service.Name == normalizedName))
            return Conflict(new { message = "A service with this name already exists." });

        var service = new Services
        {
            Name = normalizedName,
            Description = dto.Description.Trim(),
            Price = dto.Price,
            DepartmentId = dto.DepartmentId
        };

        _context.Services.Add(service);
        await _context.SaveChangesAsync();

        return StatusCode(StatusCodes.Status201Created, new
        {
            message = "Service created successfully.",
            serviceId = service.Id
        });
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ServiceDto>>> GetServices([FromQuery] string? name)
    {
        var query = _context.Services.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(name))
            query = query.Where(service => service.Name.Contains(name.Trim()));

        var services = await query
            .OrderBy(service => service.Name)
            .Select(service => new ServiceDto
            {
                Id = service.Id,
                Name = service.Name,
                Description = service.Description,
                Price = service.Price,
                DepartmentId = service.DepartmentId,
                DepartmentName = service.Department!.Name
            })
            .ToListAsync();

        return Ok(services);
    }
}