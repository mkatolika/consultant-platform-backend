using System.ComponentModel.DataAnnotations;

namespace ConsultationApplication.DTOs;

public class BookingDto
{
    [Required, StringLength(450)]
    public string ConsultantId { get; set; } = string.Empty;

    [Range(1, int.MaxValue)]
    public int ServiceId { get; set; }

    [Range(1, int.MaxValue)]
    public int SlotId { get; set; }
}