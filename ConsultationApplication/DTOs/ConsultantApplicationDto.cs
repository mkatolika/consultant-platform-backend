using System.ComponentModel.DataAnnotations;

namespace ConsultationApplication.DTOs;

public class ConsultantApplicationDto
{
    [Required, StringLength(120, MinimumLength = 2)]
    public string Specialization { get; set; } = string.Empty;

    [Required, StringLength(200, MinimumLength = 2)]
    public string Qualification { get; set; } = string.Empty;

    [Required, StringLength(100, MinimumLength = 3)]
    public string LicenseNumber { get; set; } = string.Empty;

    [Range(0, 70)]
    public int YearsOfExperience { get; set; }

    [Required, MinLength(1)]
    public List<int> ServiceIds { get; set; } = [];
}
