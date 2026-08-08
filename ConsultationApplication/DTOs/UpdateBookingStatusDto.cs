using static ConsultationApplication.Models.Bookings;

namespace ConsultationApplication.DTOs
{
    
    public class UpdateBookingStatusDto
    {
        public BookingStatus Status { get; set; }
    }

}
