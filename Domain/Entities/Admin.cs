namespace Domain.Entities
{
    public class Admin
    {
        public string Id { get; set; } = string.Empty;
        public int CompanyId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string? ProfilePictureUrl { get; set; }
        public string Department { get; set; } = string.Empty;
        public string AddedById { get; set; } = string.Empty;
        public DateTime DateCreated { get; set; } = DateTime.UtcNow;
        public DateTime? LastLogin { get; set; }
        
        // Navigation properties
        public AppUser AppUser { get; set; } = null!;
        public Company Company { get; set; } = null!;
        public SuperAdmin AddedBy { get; set; } = null!;
    }
}