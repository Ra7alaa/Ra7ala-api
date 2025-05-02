namespace Infrastructure.Data.DataSeed
{
    internal class AdminSeedDto
    {
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public int CompanyId { get; set; }
        public string Department { get; set; } = string.Empty;
    }
}