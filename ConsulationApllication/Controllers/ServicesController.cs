using ConsulationApplication.Data;
using ConsulationApplication.DTOs;
using ConsulationApplication.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace ConsulationApplication.Controllers
{
    
    [ApiController]
    [Route("api/[controller]")]
    public class ServicesController : ControllerBase
    {
        private readonly ConsulatationAppDBContext _context;

        public ServicesController(ConsulatationAppDBContext context)
        {
            _context = context;
        }

        // POST: api/Services/create
        [HttpPost("create")]
        public async Task<IActionResult> CreateService([FromBody] Services service)
        {
            _context.Services.Add(service);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Service created successfully", serviceId = service.Id });
        }


        [HttpGet]
        public async Task<IActionResult> GetServices([FromQuery] string? name)
        {
            var services = _context.Services
                .Include(s => s.Department)
                .AsQueryable();

            if (!string.IsNullOrEmpty(name))
            {
                services = services.Where(s => s.Name.Contains(name));
            }

            var result = await services
                .Select(s => new ServiceDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    Description = s.Description,
                    Price = s.Price,
                    DepartmentId = s.Department.Id,
                    DepartmentName = s.Department.Name
                })
                .ToListAsync();

            return Ok(result);
        }
    }
}
