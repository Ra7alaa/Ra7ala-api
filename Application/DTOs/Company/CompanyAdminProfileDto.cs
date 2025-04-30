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
    
    public ICollection<DriverDetailsDto> Drivers { get; set; } = new List<DriverDetailsDto>();
    public ICollection<BusDetailsDto> Buses { get; set; } = new List<BusDetailsDto>();
    public ICollection<RouteDetailsDto> Routes { get; set; } = new List<RouteDetailsDto>();
    public ICollection<FeedbackDetailsDto> Feedbacks { get; set; } = new List<FeedbackDetailsDto>();
}

