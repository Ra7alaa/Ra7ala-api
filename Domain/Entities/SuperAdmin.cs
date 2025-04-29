namespace Domain.Entities
{
    public class SuperAdmin : BaseEntity<string>
    {
        public int CompanyId { get; set; }
        
        // Navigation properties
        public AppUser AppUser { get; set; } = null!;
        public Company Company { get; set; } = null!;
        public ICollection<Admin> AddedAdmins { get; set; } = new List<Admin>();
    }
}