using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ConsultationApplication.Models;

public class Department
{
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    [JsonIgnore]
    public ICollection<Services> Services { get; set; } = [];
}