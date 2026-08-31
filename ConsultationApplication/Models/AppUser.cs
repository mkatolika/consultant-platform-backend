using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace ConsultationApplication.Models;

public class AppUser : IdentityUser
{
    [Required, StringLength(150)]
    public string FullName { get; set; } = string.Empty;

    [Url]
    public string? PhotoUrl { get; set; }

    public ICollection<Bookings> ClientBookings { get; set; } = [];
    public ICollection<Bookings> ConsultantBookings { get; set; } = [];
    public ICollection<Slot> Slots { get; set; } = [];
}