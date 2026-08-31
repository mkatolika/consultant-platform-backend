using System.ComponentModel.DataAnnotations;

namespace ConsultationApplication.Models;

public class Bookings
{
    public int Id { get; set; }

    [Required]
    public string ClientId { get; set; } = string.Empty;
    public AppUser Client { get; set; } = null!;

    [Required]
    public string ConsultantId { get; set; } = string.Empty;
    public AppUser Consultant { get; set; } = null!;

    public int ServiceId { get; set; }
    public Services Service { get; set; } = null!;

    public int SlotId { get; set; }
    public Slot Slot { get; set; } = null!;

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