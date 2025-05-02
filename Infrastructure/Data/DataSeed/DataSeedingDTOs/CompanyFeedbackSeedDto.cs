namespace Infrastructure.Data.DataSeed
{
    internal class CompanyFeedbackSeedDto
    {
        public string PassengerId { get; set; } = string.Empty;
        public int CompanyId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}