using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ConsultationApplication.Models;

public class Services
{
    public int Id { get; set; }

    [Required, MaxLength(120)]
    public string Name { get; set; } = string.Empty;

    [Required, MaxLength(1000)]
    public string Description { get; set; } = string.Empty;

    [Range(typeof(decimal), "0.01", "1000000")]
    public decimal Price { get; set; }

    public int DepartmentId { get; set; }

    [JsonIgnore]
    public Department? Department { get; set; }

    [JsonIgnore]
    public ICollection<Bookings> Bookings { get; set; } = [];

    [JsonIgnore]
    public ICollection<ConsultantService> ConsultantServices { get; set; } = [];
}