namespace Infrastructure.Data.DataSeed
{
    internal class BusSeedDto
    {
        public int Id { get; set; }
        public string RegistrationNumber { get; set; } = string.Empty;
        public int Capacity { get; set; }
        public string Model { get; set; } = string.Empty;
        public int YearOfManufacture { get; set; }
        public bool IsActive { get; set; }
        public int CompanyId { get; set; }
        public string AmenityDescription { get; set; } = string.Empty;
    }
}