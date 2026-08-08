namespace ConsulationApplication.DTOs
{
    
    public class BookingDto
    {
        public string UserId { get; set; }
        public string ConsultantId { get; set; }   // string because it ties to AspNetUsers
        public int ServiceId { get; set; }         
        public int SlotId { get; set; }
    }
}
