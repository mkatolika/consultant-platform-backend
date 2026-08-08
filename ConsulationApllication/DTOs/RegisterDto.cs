using System.Text.Json.Serialization;

namespace ConsulationApplication.DTOs
{
    public class RegisterDto
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }

       
    }
}
