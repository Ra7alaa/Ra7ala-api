using Application.DTOs.Company;

public class CompanyAdminProfileDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string LogoUrl { get; set; } = string.Empty;
    public double AverageRating { get; set; }
    public int TotalRatings { get; set; }
    public string SuperAdminName { get; set; } = string.Empty;
    public string SuperAdminEmail { get; set; } = string.Empty;
    public string SuperAdminPhone { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
    public DateTime? ApprovedDate { get; set; }
    public List<AdminInfoDto> Admins { get; set; } = new();
}

public class AdminInfoDto
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
}