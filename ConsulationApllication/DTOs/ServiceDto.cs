namespace ConsulationApplication.DTOs
{
    
    public class ServiceDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }

        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
    }
}
