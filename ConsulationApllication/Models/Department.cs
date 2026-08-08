using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ConsulationApplication.Models
{

    public class Department
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        public string? Description { get; set; }


        [JsonIgnore]
        public ICollection<Services> Services { get; set; } = new List<Services>();

    }
}
