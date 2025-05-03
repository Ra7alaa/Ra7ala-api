namespace Application.DTOs.Auth
{
    public class PassengerProfileDto
    {
        public string Id { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string? ProfilePictureUrl { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string Address { get; set; } = string.Empty;
        public DateTime DateCreated { get; set; }
        public DateTime? LastLogin { get; set; }
        // public string? TravelPreferences { get; set; }
        // public bool IsVerified { get; set; }
        // public int? TotalTrips { get; set; }
        public string UserType { get; set; } = "Passenger";
    }
}