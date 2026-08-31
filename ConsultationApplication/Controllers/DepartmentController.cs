using ConsultationApplication.Data;
using ConsultationApplication.DTOs;
using ConsultationApplication.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ConsultationApplication.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DepartmentController : ControllerBase
{
    private readonly ConsultationAppDbContext _context;

    public DepartmentController(ConsultationAppDbContext context)
    {
        _context = context;
    }

    [HttpPost("create")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateDepartment([FromBody] CreateDepartmentDto dto)
    {
        var normalizedName = dto.Name.Trim();
        if (await _context.Departments.AnyAsync(department => department.Name == normalizedName))
            return Conflict(new { message = "A department with this name already exists." });

        var department = new Department
        {
            Name = normalizedName,
            Description = dto.Description?.Trim()
        };

        _context.Departments.Add(department);
        await _context.SaveChangesAsync();

        return StatusCode(StatusCodes.Status201Created, new
        {
            message = "Department created successfully.",
            departmentId = department.Id
        });
    }
}