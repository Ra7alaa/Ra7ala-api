namespace Domain.Entities{
    public class Ticket : BaseEntity<int>
    {
        public int TripId { get; set; }
        public string PassengerId { get; set; } = string.Empty; 
        public decimal Price { get; set; }
        public DateTime PurchaseDate { get; set; } = DateTime.UtcNow;
        public bool IsUsed { get; set; } = false;

        // Navigation properties
        public virtual Trip Trip { get; set; } = null!;
        public virtual AppUser Passenger { get; set; } = null!;
    }
}