namespace ConsultationApplication.Models;

public class ConsultantService
{
    public int ConsultantId { get; set; }
    public Consultant Consultant { get; set; } = null!;
    public int ServiceId { get; set; }
    public Services Service { get; set; } = null!;
}