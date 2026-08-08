namespace ConsulationApplication.Models
{

    public class ConsultantApplicationDto
    {
        public string Specialization { get; set; }
        public string Qualification { get; set; }
        public string LicenseNumber { get; set; }
        public int YearsOfExperience { get; set; }
        public List<int> ServiceIds { get; set; } // services they want to provide
    }
    public class ConsultantApprovalDto
    {
        public int ConsultantId { get; set; }
    }


}
