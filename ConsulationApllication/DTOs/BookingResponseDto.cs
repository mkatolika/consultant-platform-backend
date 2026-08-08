using static ConsulationApplication.Models.Bookings;

namespace ConsulationApplication.DTOs
{
    
    public class BookingResponseDto
    {
        public int Id { get; set; }
        public string ClientName { get; set; }
        public string ConsultantName { get; set; }
        public string ServiceName { get; set; }
        public DateTime SlotStart { get; set; }
        public DateTime SlotEnd { get; set; }
        public BookingStatus Status { get; set; }
    }
}
