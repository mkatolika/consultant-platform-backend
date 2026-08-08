using Microsoft.AspNetCore.Identity;
namespace ConsulationApplication.Models
{
    public class AppUser:IdentityUser

    {

        public string FullName { get; set; }
       public string?  PhotoUrl { get;set; }

        // Navigation
        public ICollection<Bookings> ClientBookings { get; set; }
            public ICollection<Bookings> ConsultantBookings { get; set; }
            public ICollection<Slot> Slots { get; set; } // consultant’s availability
        
    }
}
