using System.Text.Json.Serialization;

namespace ConsulationApplication.Models
{
    public class Consultant
    {
        public int Id { get; set; }
        public string UserId { get; set; }

        [JsonIgnore]
        public AppUser User { get; set; }

        public string Specialization { get; set; }
        public string Qualification { get; set; }
        public string LicenseNumber { get; set; }
        public int YearsOfExperience { get; set; }
        public int Rating { get; set; }

        public bool IsApproved { get; set; } = false;

        // Link to services via join table
        
        [JsonIgnore]
        public ICollection<ConsultantService> ConsultantServices { get; set; }
    }
}
