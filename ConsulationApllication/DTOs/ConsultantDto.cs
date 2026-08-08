namespace ConsulationApplication.DTOs
{
    
    public class ConsultantDto
    {
        public string UserId { get; set; }
        public string FullName { get; set; }   // from User
        public string PhotoUrl { get; set; }   // from User
        public string Specialization { get; set; }
        public string Qualification { get; set; }
        public string LicenseNumber { get; set; }
        public int YearsOfExperience { get; set; }
        public double Rating { get; set; }
    }
}
