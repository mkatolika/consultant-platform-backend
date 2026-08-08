using ConsulationApplication.Data;
using ConsulationApplication.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ConsulationApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentController : ControllerBase
    {
        private readonly ConsulatationAppDBContext _context;

        public DepartmentController(ConsulatationAppDBContext context)
        {
            _context = context;
        }
        [HttpPost("create")]
        public async Task<IActionResult> CreateDepartment([FromBody] Department department)
        {
            if (string.IsNullOrWhiteSpace(department.Name))
                return BadRequest("Department name is required.");

            _context.Departments.Add(department);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Department created successfully", department });

        }
    }
}
