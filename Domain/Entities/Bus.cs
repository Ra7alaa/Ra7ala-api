using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class Bus : BaseEntity<int>
    {
        [Required]
        [StringLength(50)]
        public string RegistrationNumber { get; set; } = string.Empty;
        public int Capacity { get; set; }
        public string Model { get; set; } = string.Empty;
        public int YearOfManufacture { get; set; }
         //public BusStatus Status { get; set; }= BusStatus.Active;
        //public string Status { get; set; } = BusStatus.Active.ToString();
       // public Point CurrentLocation { get; set; } // Real-time location
        public bool IsActive { get; set; } = true;
        public int CompanyId { get; set; }
        public string AmenityDescription { get; set; } = string.Empty;
       
        // Navigation properties
        public virtual Company Company { get; set; } = null!;

        // public virtual ICollection<Trip> Trips { get; set; } = new List<Trip>();
       // public virtual ICollection<Seat> Seats { get; set; } = new List<Seat>();
 
    }
}