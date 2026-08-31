using static ConsultationApplication.Models.Bookings;

namespace ConsultationApplication.DTOs;

public class BookingResponseDto
{
    public int Id { get; set; }
    public string ClientName { get; set; } = string.Empty;
    public string ConsultantName { get; set; } = string.Empty;
    public string ServiceName { get; set; } = string.Empty;
    public DateTime SlotStart { get; set; }
    public DateTime SlotEnd { get; set; }
    public BookingStatus Status { get; set; }
}