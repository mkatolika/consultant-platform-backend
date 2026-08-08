using System.Reflection.Metadata;
using System.Text.Json.Serialization;

namespace ConsulationApplication.Models
{
    public class Services
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int DepartmentId { get; set; }

        [JsonIgnore]
        public Department? Department { get; set; }

        [JsonIgnore]
        public ICollection<Bookings> Bookings { get; set; } = new List<Bookings>();

        [JsonIgnore]
        public ICollection<ConsultantService> ConsultantServices { get; set; } = new List<ConsultantService>();

        


    }
}
