using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ConsultationApplication.Models;

public class Consultant
{
    public int Id { get; set; }

    [Required]
    public string UserId { get; set; } = string.Empty;

    [JsonIgnore]
    public AppUser User { get; set; } = null!;

    [Required, MaxLength(120)]
    public string Specialization { get; set; } = string.Empty;

    [Required, MaxLength(200)]
    public string Qualification { get; set; } = string.Empty;

    [Required, MaxLength(100)]
    public string LicenseNumber { get; set; } = string.Empty;

    [Range(0, 70)]
    public int YearsOfExperience { get; set; }

    [Range(0, 5)]
    public int Rating { get; set; }

    public bool IsApproved { get; set; }

    [JsonIgnore]
    public ICollection<ConsultantService> ConsultantServices { get; set; } = [];
}