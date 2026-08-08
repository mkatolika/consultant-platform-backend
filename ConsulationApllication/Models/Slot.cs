using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ConsulationApplication.Models
{
    
    public class Slot
    {
        public int Id { get; set; }

        // Consultant who owns this slot
        [Required]
       
        public string ConsultantId { get; set; }
        [JsonIgnore]
        public AppUser? Consultant { get; set; }

        // Start and end time of the slot
        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        public DateTime EndTime { get; set; }

        // Is this slot available for booking?
        public bool IsAvailable { get; set; } = true;

        // Navigation: which booking reserved this slot
        [JsonIgnore]
        public Bookings? Booking { get; set; }
    }

}
