namespace Infrastructure.Data.DataSeed
{
    internal class CompanySeedDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string LogoUrl { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public decimal AverageRating { get; set; }
        public int TotalRatings { get; set; }
        public SuperAdminSeedDto? SuperAdmin { get; set; }
    }
}
