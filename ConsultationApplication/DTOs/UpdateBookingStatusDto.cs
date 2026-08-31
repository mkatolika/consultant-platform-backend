using System.ComponentModel.DataAnnotations;
using static ConsultationApplication.Models.Bookings;

namespace ConsultationApplication.DTOs;

public class UpdateBookingStatusDto
{
    [EnumDataType(typeof(BookingStatus))]
    public BookingStatus Status { get; set; }
}