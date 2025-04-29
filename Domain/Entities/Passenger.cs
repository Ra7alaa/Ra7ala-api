namespace Domain.Entities
{
    public class Passenger : BaseEntity<string>
    {
        // Properties


        // Navigation property
        public AppUser AppUser { get; set; } = null!;
    }
}