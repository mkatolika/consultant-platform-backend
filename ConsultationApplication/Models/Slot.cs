using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ConsultationApplication.Models;

public class Slot
{
    public int Id { get; set; }

    [Required]
    public string ConsultantId { get; set; } = string.Empty;

    [JsonIgnore]
    public AppUser? Consultant { get; set; }

    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public bool IsAvailable { get; set; } = true;

    [JsonIgnore]
    public Bookings? Booking { get; set; }
}