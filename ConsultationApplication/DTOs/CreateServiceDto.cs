using System.ComponentModel.DataAnnotations;

namespace ConsultationApplication.DTOs;

public class CreateServiceDto
{
    [Required, StringLength(120, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;

    [Required, StringLength(1000, MinimumLength = 10)]
    public string Description { get; set; } = string.Empty;

    [Range(typeof(decimal), "0.01", "1000000")]
    public decimal Price { get; set; }

    [Range(1, int.MaxValue)]
    public int DepartmentId { get; set; }
}