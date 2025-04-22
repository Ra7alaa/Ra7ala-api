using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class Trip
    {
        [Key]
        public int Id { get; set; }
        public int RouteId { get; set; }
        public int BusId { get; set; }
        public int? DriverId { get; set; }
        public DateTime DepartureDate { get; set; }
        public string Status { get; set; } // Scheduled, In Progress, Completed, Cancelled
        public decimal Price { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual Route Route { get; set; }
        public virtual Bus Bus { get; set; }
        public virtual Driver Driver { get; set; }
       // public virtual ICollection<TripStation> TripStations { get; set; }
       // public virtual ICollection<Ticket> Tickets { get; set; }
       // public virtual ICollection<Feedback> Feedbacks { get; set; }
    }
}