using System.ComponentModel.DataAnnotations;

namespace ConsulationApplication.Models
{

    public class Bookings
    {
        public int Id { get; set; }

        [Required]
        public string ClientId { get; set; }
        public AppUser Client { get; set; }

        [Required]
        public string ConsultantId { get; set; }
        public AppUser Consultant { get; set; }

        [Required]
        public int ServiceId { get; set; }
        public Services Service { get; set; }

        // Instead of free DateTime, link to Slot
        [Required]
        public int SlotId { get; set; }
        public Slot Slot { get; set; }


        public BookingStatus Status { get; set; } = BookingStatus.Pending;

        public enum BookingStatus
    {
        Pending,
        Accepted,
        Rejected,
        Cancelled,
        Completed
    }
    }
}
