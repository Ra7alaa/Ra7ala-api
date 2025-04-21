namespace Domain.Entities
{
    public class SuperAdmin
    {
        public string Id { get; set; } = string.Empty;
        public int CompanyId { get; set; }
        
        // Navigation properties
        public AppUser AppUser { get; set; } = null!;
        public Company Company { get; set; } = null!;
        public ICollection<Admin> AddedAdmins { get; set; } = new List<Admin>();
    }
}