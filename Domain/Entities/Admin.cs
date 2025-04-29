namespace Domain.Entities
{
    public class Admin : BaseEntity<string>
    {
        public int CompanyId { get; set; }
        public string Department { get; set; } = string.Empty;
        public string AddedById { get; set; } = string.Empty;
        
        // Navigation properties
        public AppUser AppUser { get; set; } = null!;
        public Company Company { get; set; } = null!;
        public SuperAdmin AddedBy { get; set; } = null!;
    }
}