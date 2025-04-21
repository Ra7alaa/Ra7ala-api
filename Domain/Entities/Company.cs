namespace Domain.Entities
{
    public class Company
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        // // Super Admin details
        // public string SuperAdminName { get; set; } = string.Empty;
        // public string SuperAdminPhone { get; set; } = string.Empty;
        // public string SuperAdminPhone { get; set; } = string.Empty;

        public string LogoUrl { get; set; } = string.Empty;
        public bool IsApproved { get; set; } = false;

        public bool IsRejected { get; set; } = false;
        public bool IsDeleted { get; set; } = false;
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? ApprovedDate { get; set; }

        // Navigation properties
        public SuperAdmin SuperAdmin { get; set; }= null!;
        public ICollection<Admin> Admins { get; set; } = new List<Admin>();
        public ICollection<Driver> Drivers { get; set; } = new List<Driver>();
    }
}