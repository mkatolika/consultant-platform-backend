namespace ConsulationApplication.Models
{
    public class ConsultantService
    {

        public int Id { get; set; }

        public int ConsultantId { get; set; }
        public Consultant Consultant { get; set; }

        public int ServiceId { get; set; }
        public Services Service { get; set; }

    }
}
