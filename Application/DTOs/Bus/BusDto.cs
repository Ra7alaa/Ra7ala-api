namespace Application.DTOs.Bus
{
    public class BusDto
    {
        public int Id { get; set; }
        public string RegistrationNumber { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public int Capacity { get; set; }
        public string AmenityDescription { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public int CompanyId { get; set; }
    }
}
