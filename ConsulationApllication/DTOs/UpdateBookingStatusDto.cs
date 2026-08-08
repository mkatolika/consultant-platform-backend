using static ConsulationApplication.Models.Bookings;

namespace ConsulationApplication.DTOs
{
    
    public class UpdateBookingStatusDto
    {
        public BookingStatus Status { get; set; }
    }

}
